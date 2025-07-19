using REBoP.Models;

namespace REBoP.Builder;

public class TaxDetailItemBuilder
{
    public decimal? Net { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Gross { get; set; }

    public TaxBracketAlias? BracketAlias { get; set; }

    public string? PartnerCode { get; set; }

    public TaxDetailItem Build()
    {
        if (BracketAlias is null)
            throw new MissingRequiredFieldBuilderException(nameof(BracketAlias));

        if (Net is not { } net)
            throw new MissingRequiredFieldBuilderException(nameof(Net));

        if (Tax is not { } tax)
            throw new MissingRequiredFieldBuilderException(nameof(Tax));

        if (Gross is not { } gross)
            throw new MissingRequiredFieldBuilderException(nameof(Gross));

        var item = new TaxDetailItem
        {
            Id = Guid.NewGuid(),
            BracketAlias = BracketAlias,
            Net = net,
            Tax = tax,
            Gross = gross,
            PartnerCode = PartnerCode,
        };

        return item;
    }

    public TaxTotalItem BuildTotal()
    {
        if (Net is not { } net)
            throw new MissingRequiredFieldBuilderException(nameof(Net));

        if (Tax is not { } tax)
            throw new MissingRequiredFieldBuilderException(nameof(Tax));

        if (Gross is not { } gross)
            throw new MissingRequiredFieldBuilderException(nameof(Gross));

        var item = new TaxTotalItem
        {
            Id = Guid.NewGuid(),
            Net = net,
            Tax = tax,
            Gross = gross,
            PartnerCode = PartnerCode,
        };

        return item;
    }
}
