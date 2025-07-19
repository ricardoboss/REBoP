namespace REBoP.Services;

public class ListReceiptContent(IPdfSource source, IReadOnlyList<string> lines) : IReceiptContent
{
    public IPdfSource Source => source;

    public IReceiptLines Lines { get; } = new ListReceiptLines(lines);
}
