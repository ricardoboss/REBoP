using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// A single line of tax details on a <see cref="Receipt"/>.
/// </summary>
[DebuggerDisplay("Bracket: {BracketAlias}, Net: {Net}, Tax: {Tax}, Gross: {Gross}")]
public class TaxDetailItem : TaxTotalItem
{
    /// <summary>
    /// Which tax bracket applies to this item.
    /// </summary>
    public required TaxBracketAlias BracketAlias { get; init; }
}
