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

    /// <summary>
    /// Whether this payment item was used to pay the receipt total.
    /// </summary>
    /// <remarks>
    /// In cases where this is <see langword="false" />, it means this item has to do with the payment, but does not
    /// contribute to paying the total. It might even add to it, which must be paid with other payment items.
    /// One such example is when taking a cash payout, which will then be paid by card.
    /// </remarks>
    public bool IsPaying => Label.StartsWith("Geg. ", StringComparison.Ordinal);

    /// <summary>
    /// Whether this item represents a cash payment.
    /// </summary>
    public bool IsCash => Label.Contains("BAR", StringComparison.Ordinal);

    /// <summary>
    /// Whether this item represents change from a cash payment.
    /// </summary>
    public bool IsChange => "Rückgeld BAR".Equals(Label, StringComparison.Ordinal);
}
