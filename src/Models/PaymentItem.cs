using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// Information about how a <see cref="Receipt"/> was paid.
/// </summary>
[DebuggerDisplay("{Label} EUR {Total}")]
public class PaymentItem
{
    /// <summary>
    /// The unique ID of this payment item.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The label of the payment item.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// The total that was paid with this item. This can be negative (pay out, voucher).
    /// </summary>
    public required decimal Total { get; init; }
}
