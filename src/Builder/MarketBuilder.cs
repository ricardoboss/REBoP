using REBoP.Extensions;
using REBoP.Models;

namespace REBoP.Builder;

public class MarketBuilder
{
    public string? Address { get; set; }

    public string? VatId { get; set; }

    public string? MarketNumber { get; set; }

    public ICollection<Receipt> Receipts { get; set; } = [];

    public Market Build()
    {
        if (Address is null)
            throw new MissingRequiredFieldBuilderException(nameof(Address));

        if (VatId is null)
            throw new MissingRequiredFieldBuilderException(nameof(VatId));

        var market = new Market
        {
            Id = Guid.NewGuid(),
            Address = Address,
            VatId = VatId,
            MarketNumber = MarketNumber,
        };

        market.Receipts.AddRange(Receipts);

        return market;
    }
}
