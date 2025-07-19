using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// A class of tax applied to <see cref="ReceiptItem"/>s individually.
/// </summary>
[DebuggerDisplay("{Multiplier}")]
public class TaxBracket
{
    /// <summary>
    /// The unique ID of this tax bracket.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The multiplier of this bracket (e.g. 19%).
    /// </summary>
    public required decimal Multiplier { get; init; }
}
