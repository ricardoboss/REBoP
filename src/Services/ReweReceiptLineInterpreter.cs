using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using REBoP.Builder;
using REBoP.Extensions;
using REBoP.Models;

namespace REBoP.Services;

public partial class ReweReceiptLineInterpreter : IReceiptLineInterpreter
{
    public Task<Receipt> ProcessAsync(IReceiptContent content, CancellationToken cancellationToken = default)
    {
        var receiptBuilder = new ReceiptBuilder
        {
            Market = ReadMarket(content),
            Total = ReadTotal(content),
        };

        var (taxTotal, taxDetailItems, partnerTaxTotals) = ReadTaxDetails(content);
        receiptBuilder.TaxTotal = taxTotal;
        receiptBuilder.TaxDetails = taxDetailItems;
        receiptBuilder.PartnerTaxTotals = partnerTaxTotals;

        var (timestamp, receiptNo, traceNo) = ReadMetadata(content);
        receiptBuilder.Timestamp = timestamp;
        receiptBuilder.ReceiptNumber = receiptNo;
        receiptBuilder.TraceNumber = traceNo;

        var receipt = receiptBuilder.Build();

        receipt.Items.AddRange(ReadItems(content, receipt));

        return Task.FromResult(receipt);
    }

    private static IEnumerable<ReceiptItem> ReadItems(IReceiptContent content, Receipt receipt)
    {
        var items = string.Join('\n', content.FindLines(
            (line, _) => line.EndsWith("EUR", StringComparison.Ordinal),
            (line, _) => line.Length > 3 && line.Skip(1).All(c => c == '-')
        ).Skip(1));

        // collect all tax brackets that can be applied to line items
        // there are brackets with "Coupon", which are not an alias and need to be excluded here
        var taxBrackets = receipt.TaxDetails
            .Select(d => d.BracketAlias)
            .Where(a => a.Alias.Length == 1)
            .DistinctBy(a => a.Alias)
            .ToDictionary(
                a => a.Alias,
                a => a
            );

        var lineNumber = 0;
        foreach (Match match in ItemLineRegex.Matches(items))
        {
            lineNumber++;

            var taxBracketAlias = match.Groups["taxBracketAlias"].Value;
            var label = match.Groups["label"].Value;
            var unit = match.Groups["unit"].Value;
            var partnerCode = match.Groups["partnerCode"].Value;

            var quantityStr = match.Groups["quantity"].Value;
            var quantity = string.IsNullOrWhiteSpace(quantityStr) ? 1m : ParseGermanDecimal(quantityStr);

            var lineTotalStr = match.Groups["lineTotal"].Value;
            var lineTotal = ParseGermanDecimal(lineTotalStr);

            var unitPriceStr = match.Groups["unitPrice"].Value;
            decimal? unitPrice = string.IsNullOrWhiteSpace(unitPriceStr) ? null : ParseGermanDecimal(unitPriceStr);

            var isNonDiscountable = !string.IsNullOrWhiteSpace(match.Groups["nonDiscountableMarker"].Value);

            var builder = new ReceiptItemBuilder
            {
                LineNumber = lineNumber,
                Receipt = receipt,
                TaxBracket = taxBrackets[taxBracketAlias],
                Label = label,
                Quantity = quantity,
                LineTotal = lineTotal,
                Unit = string.IsNullOrWhiteSpace(unit) ? null : unit,
                UnitPrice = unitPrice,
                IsNonDiscountable = isNonDiscountable,
                PartnerCode = string.IsNullOrWhiteSpace(partnerCode) ? null : partnerCode,
            };

            yield return builder.Build();
        }
    }

    [GeneratedRegex(
        @"^(?<label>.+?)\s{2,}(?<partnerCode>\w+)?\s{2,}(?<lineTotal>-?\d+,\d+)\s(?<taxBracketAlias>\w)(?:\s(?<nonDiscountableMarker>\*))?(?:\n\s+(?<quantity>-?\d+(?:,\d+)?)\s+(?<unit>\S+)\s+x\s+(?<unitPrice>-?\d+,\d+))?$",
        RegexOptions.Multiline)]
    private static partial Regex ItemLineRegex { get; }

    private static (DateTimeOffset timestamp, int receiptNo, int? traceNo) ReadMetadata(IReceiptContent content)
    {
        string dateStr;
        string timeStr;
        string receiptNoStr;
        string? traceNoStr;
        if (content.Lines.Any(l => l.StartsWith("Datum: ", StringComparison.Ordinal)))
        {
            // newer receipt layout
            var (dateLineIdx, dateLine) = content.Lines.Index().First(t => t.Item.StartsWith("Datum: ", StringComparison.Ordinal));
            var timeLine = content.Lines[dateLineIdx + 1];
            var receiptNumberLine = content.Lines[dateLineIdx + 2];
            var traceNumberLine = content.Lines[dateLineIdx + 3];

            dateStr = dateLine[(dateLine.LastIndexOf(' ') + 1)..];
            timeLine = timeLine[..^" Uhr".Length];
            timeStr = timeLine[(timeLine.LastIndexOf(' ') + 1)..];
            receiptNoStr = receiptNumberLine[(receiptNumberLine.LastIndexOf(' ') + 1)..];
            traceNoStr = traceNumberLine[(traceNumberLine.LastIndexOf(' ') + 1)..];
        }
        else
        {
            // older receipt layout
            var dateTimeReceiptLine = content.Lines.First(l => l.Contains("Bon-Nr.:", StringComparison.Ordinal));

            var components = dateTimeReceiptLine.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            dateStr = components[0];
            timeStr = components[1];
            receiptNoStr = components[2]["Bon-Nr.:".Length..];
            traceNoStr = null;
        }

        var date = DateOnly.Parse(dateStr, GermanCulture);
        var time = TimeOnly.Parse(timeStr, GermanCulture);
        var timestamp = ToGermanDateTimeOffset(date, time);

        var receiptNo = int.Parse(receiptNoStr, GermanCulture);
        int? traceNo = string.IsNullOrWhiteSpace(traceNoStr) ? null : int.Parse(traceNoStr, GermanCulture);

        return (timestamp, receiptNo, traceNo);
    }

    private static DateTimeOffset ToGermanDateTimeOffset(DateOnly date, TimeOnly time)
    {
        var localDateTime = date.ToDateTime(time);

        var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "W. Europe Standard Time" // Windows ID
            : "Europe/Berlin"; // IANA ID (Linux/macOS)

        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return new(localDateTime, tz.GetUtcOffset(localDateTime));
    }

    private static decimal ReadTotal(IReceiptContent content)
    {
        var sumLine = content.FindLine((line, _) => line.TrimStart().StartsWith("SUMME ", StringComparison.Ordinal))?.TrimStart();
        if (sumLine is null)
            throw new ReceiptInterpreterException("Did not find the total sum line");

        var sumStr = sumLine[(sumLine.LastIndexOf(' ') + 1)..];

        return ParseGermanDecimal(sumStr);
    }

    private static (TaxTotalItem total, ICollection<TaxDetailItem> details, ICollection<TaxTotalItem> partnerTaxTotals)
        ReadTaxDetails(IReceiptContent content)
    {
        var tableLines = content.FindLines(
                (line, _) => line.TrimStart().StartsWith("Steuer", StringComparison.Ordinal),
                (line, _) => line.TrimStart().StartsWith("Gesamtbetrag", StringComparison.Ordinal)
            )
            .Skip(1) // Skip header line
            ;

        List<TaxDetailItem> details = [];
        List<TaxTotalItem> partnerTotals = [];
        TaxTotalItem? total = null;
        string? partnerCode = null;
        foreach (var line in tableLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.Contains("Konzessionär:", StringComparison.Ordinal))
            {
                partnerCode = line.TrimEnd()[(line.TrimEnd().LastIndexOf(' ') + 1)..];

                continue;
            }

            var (label, net, tax, gross) = ReadTaxDetailLine(line);
            var builder = new TaxDetailItemBuilder
            {
                Net = net,
                Tax = tax,
                Gross = gross,
                PartnerCode = partnerCode,
            };

            // handle total
            if (label == "Gesamtbetrag")
            {
                if (partnerCode is null)
                {
                    total = builder.BuildTotal();
                }
                else
                {
                    partnerTotals.Add(builder.BuildTotal());
                    partnerCode = null;
                }
            }
            else
            {
                var bracketAlias = ReadTaxBracketAlias(label);
                var knownAliases = details.Select(d => d.BracketAlias);
                if (knownAliases.FirstOrDefault(a => a.Alias == bracketAlias.Alias) is { } knownAlias)
                    bracketAlias = knownAlias;

                builder.BracketAlias = bracketAlias;

                details.Add(builder.Build());
            }
        }

        if (total is null)
            throw new ReceiptInterpreterException("No total was found in the tax details section");

        return (total, details, partnerTotals);
    }

    private static TaxBracketAlias ReadTaxBracketAlias(string label)
    {
        // e.g.:
        // A=  19,0%
        // B=   7,0%
        // Coupon 19,0%
        // Coupon  7,0%
        var match = TaxBracketAliasRegex.Match(label);

        Debug.Assert(match.Success, "Tax detail label did not match expected format: " + label);

        var alias = match.Groups["alias"].Value;
        var valueStr = match.Groups["value"].Value;
        var value = ParseGermanDecimal(valueStr);

        var bracket = new TaxBracket
        {
            Id = Guid.NewGuid(),
            // value is in percentage, so divide by 100 to get numerical value
            Multiplier = value / 100m,
        };

        var bracketAlias = new TaxBracketAlias
        {
            Id = Guid.NewGuid(),
            Bracket = bracket,
            Alias = alias.TrimEnd('='),
        };

        return bracketAlias;
    }

    [GeneratedRegex(@"^(?<alias>\S+?)\s+(?<value>\d+,\d+)%$")]
    private static partial Regex TaxBracketAliasRegex { get; }

    private static (string label, decimal net, decimal tax, decimal gross) ReadTaxDetailLine(string taxTotalLine)
    {
        var match = TaxDetailRegex.Match(taxTotalLine.Trim());

        Debug.Assert(match.Success, "Tax detail line did not match expected form: " + taxTotalLine);

        var label = match.Groups["label"].Value;
        var netStr = match.Groups["net"].Value;
        var taxStr = match.Groups["tax"].Value;
        var grossStr = match.Groups["gross"].Value;

        var net = ParseGermanDecimal(netStr);
        var tax = ParseGermanDecimal(taxStr);
        var gross = ParseGermanDecimal(grossStr);

        return (label, net, tax, gross);
    }

    [GeneratedRegex(@"^\s*(?<label>.+?)\s+(?<net>-?\d+,\d+)\s+(?<tax>-?\d+,\d+)\s+(?<gross>-?\d+,\d+)$")]
    private static partial Regex TaxDetailRegex { get; }

    private static readonly CultureInfo GermanCulture = new("de-DE", useUserOverride: false);

    private static decimal ParseGermanDecimal(string netStr)
    {
        const NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        return decimal.Parse(netStr, style, GermanCulture);
    }

    private static Market ReadMarket(IReceiptContent content)
    {
        // a receipt typically has the market at the very beginning, after a few blank lines (for a logo or so)

        var marketAddressLines = content.FindLines(
            (line, _) => !string.IsNullOrWhiteSpace(line),
            (line, _) => line.Trim().EndsWith("EUR", StringComparison.Ordinal)
        );

        var builder = new MarketBuilder();
        var addressBuilder = new StringBuilder();

        foreach (var line in marketAddressLines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("UID Nr.: ", StringComparison.Ordinal))
            {
                var vatId = trimmed["UID Nr.: ".Length..];

                builder.VatId = vatId;

                // VAT-ID is the line _after_ the address
                break;
            }

            addressBuilder.AppendLine(trimmed);
        }

        builder.Address = addressBuilder.ToString().TrimEnd();

        var marketNumberLine = content.FindLine((line, _) => line.TrimStart().StartsWith("Markt:", StringComparison.Ordinal))?.TrimStart();
        if (marketNumberLine is not null)
        {
            var firstSpaceChar = marketNumberLine.IndexOf(' ', StringComparison.Ordinal);
            var marketNumber = marketNumberLine["Markt:".Length..firstSpaceChar];

            builder.MarketNumber = marketNumber;
        }

        return builder.Build();
    }
}
