using REBoP.Models;

namespace REBoP.Builder;

public class ReceiptItemBuilder
{
    public Receipt? Receipt { get; set; }

    public TaxBracketAlias? TaxBracket { get; set; }

    public string? Label { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public bool? IsNonDiscountable { get; set; }

    public int? LineNumber { get; set; }

    public string? PartnerCode { get; set; }

    public ReceiptItem Build()
    {
        if (Receipt is null)
            throw new MissingRequiredFieldBuilderException(nameof(Receipt));

        if (TaxBracket is null)
            throw new MissingRequiredFieldBuilderException(nameof(TaxBracket));

        if (Label is not { } label)
            throw new MissingRequiredFieldBuilderException(nameof(Label));

        if (Quantity is not { } quantity)
            throw new MissingRequiredFieldBuilderException(nameof(Quantity));

        if (LineTotal is not { } lineTotal)
            throw new MissingRequiredFieldBuilderException(nameof(LineTotal));

        var item = new ReceiptItem
        {
            Id = Guid.NewGuid(),
            Receipt = Receipt,
            TaxBracket = TaxBracket,
            Label = label,
            Quantity = quantity,
            LineTotal = lineTotal,
            Unit = Unit,
            UnitPrice = UnitPrice,
            IsNonDiscountable = IsNonDiscountable ?? false,
            LineNumber = LineNumber,
            PartnerCode = PartnerCode,
        };

        return item;
    }
}
