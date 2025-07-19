using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// A single line or item on a <see cref="Receipt"/>.
/// </summary>
[DebuggerDisplay("{Label} ({Quantity}{Unit} x {UnitPrice}) => {LineTotal} {TaxBracket}")]
public class ReceiptItem
{
    /// <summary>
    /// The unique ID for this item.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The receipt this item belongs to.
    /// </summary>
    public required Receipt Receipt { get; init; }

    /// <summary>
    /// Which tax bracket was used for this item.
    /// </summary>
    public required TaxBracketAlias TaxBracket { get; init; }

    /// <summary>
    /// The (1-based) line of this item on the receipt.
    /// </summary>
    public int? LineNumber { get; init; }

    /// <summary>
    /// The label of this item on the receipt.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// How much of the item was bought.
    /// </summary>
    public required decimal Quantity { get; init; } = 1m;

    /// <summary>
    /// The total price of this line on the receipt.
    /// </summary>
    public required decimal LineTotal { get; init; }

    /// <summary>
    /// The price per unit of this item.
    /// </summary>
    public decimal? UnitPrice { get; init; }

    /// <summary>
    /// The unit in which this item is measured in.
    /// </summary>
    public string? Unit { get; init; }

    /// <summary>
    /// The concessionaire/partner code for this item.
    /// </summary>
    public string? PartnerCode { get; init; }

    /// <summary>
    /// Whether this item is eligible for price reductions. The default is <see langword="false"/>
    /// </summary>
    public bool IsNonDiscountable { get; init; }
}
