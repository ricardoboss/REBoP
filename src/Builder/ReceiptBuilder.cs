using REBoP.Extensions;
using REBoP.Models;

namespace REBoP.Builder;

public class ReceiptBuilder
{
    public Market? Market { get; set; }

    public TaxTotalItem? TaxTotal { get; set; }

    public decimal? Total { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ICollection<ReceiptItem> Items { get; set; } = [];

    public ICollection<PaymentItem> Payment { get; set; } = [];

    public ICollection<TaxDetailItem> TaxDetails { get; set; } = [];

    public ICollection<TaxTotalItem> PartnerTaxTotals { get; set; } = [];

    public int? ReceiptNumber { get; set; }

    public int? TraceNumber { get; set; }

    public Receipt Build()
    {
        if (Market is null)
            throw new MissingRequiredFieldBuilderException(nameof(Market));

        if (TaxTotal is null)
            throw new MissingRequiredFieldBuilderException(nameof(TaxTotal));

        if (Total is not { } total)
            throw new MissingRequiredFieldBuilderException(nameof(Total));

        if (Timestamp is not { } timestamp)
            throw new MissingRequiredFieldBuilderException(nameof(Timestamp));

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            Market = Market,
            TaxTotal = TaxTotal,
            Total = total,
            Timestamp = timestamp,
            ReceiptNumber = ReceiptNumber,
            TraceNumber = TraceNumber,
        };

        receipt.Items.AddRange(Items);
        receipt.Payment.AddRange(Payment);
        receipt.TaxDetails.AddRange(TaxDetails);
        receipt.PartnerTaxTotals.AddRange(PartnerTaxTotals);

        return receipt;
    }
}
