using System.Diagnostics;

namespace REBoP.Models;

[DebuggerDisplay("{Address}")]
public class Market
{
    public required Guid Id { get; init; }

    public ICollection<Receipt> Receipts { get; } = [];

    public required string Address { get; init; }

    public required string VatId { get; init; }

    public string? MarketNumber { get; init; }
}
