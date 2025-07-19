using System.Diagnostics;

namespace REBoP.Models;

/// <summary>
/// A piece of paper holding information about where, what and when something was bought. Also includes payment
/// information.
/// </summary>
[DebuggerDisplay("Market = {Market}, Timestamp = {Timestamp}, Items = {Items}, Total = {Total}")]
public class Receipt
{
    /// <summary>
    /// The unique ID for this receipt.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The market where this receipt was printed at/where the items were bought.
    /// </summary>
    public required Market Market { get; init; }

    /// <summary>
    /// The items on this receipt.
    /// </summary>
    public ICollection<ReceiptItem> Items { get; } = [];

    /// <summary>
    /// Information about how the receipt was paid.
    /// </summary>
    public ICollection<PaymentItem> Payment { get; } = [];

    /// <summary>
    /// Details about tax paid per bracket.
    /// </summary>
    public ICollection<TaxDetailItem> TaxDetails { get; } = [];

    /// <summary>
    /// The summaries of taxes paid to partners.
    /// </summary>
    public ICollection<TaxTotalItem> PartnerTaxTotals { get; } = [];

    /// <summary>
    /// The summary of tax paid.
    /// </summary>
    public required TaxTotalItem TaxTotal { get; init; }

    /// <summary>
    /// The total of this receipt. Basically a sum of all <see cref="Items"/>'s <see cref="ReceiptItem.LineTotal"/>s.
    /// </summary>
    public required decimal Total { get; init; }

    /// <summary>
    /// The timestamp on the receipt. Includes both date and time.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// The number of this receipt for the market on this day.
    /// </summary>
    public int? ReceiptNumber { get; init; }

    /// <summary>
    /// A trace number. Use unknown.
    /// </summary>
    public int? TraceNumber { get; init; }
}
