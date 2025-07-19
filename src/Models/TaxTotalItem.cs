using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// The summary line of tax details on a <see cref="Receipt"/>.
/// </summary>
[DebuggerDisplay("Net: {Net}, Tax: {Tax}, Gross: {Gross}")]
public class TaxTotalItem
{
    /// <summary>
    /// The unique ID of this item.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// An optional partner code, in case a <see cref="ReceiptItem"/> was bought at a partnered store.
    /// </summary>
    public string? PartnerCode { get; init; }

    /// <summary>
    /// The net amount in this bracket.
    /// </summary>
    public required decimal Net { get; init; }

    /// <summary>
    /// The amount added from this tax bracket.
    /// </summary>
    public required decimal Tax { get; init; }

    /// <summary>
    /// The amount paid. Basically <see cref="Net"/> + <see cref="Tax"/>.
    /// </summary>
    public required decimal Gross { get; init; }
}
