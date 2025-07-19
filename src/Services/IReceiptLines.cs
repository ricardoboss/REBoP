namespace REBoP.Services;

public interface IReceiptLines : IReadOnlyList<string>
{
    IEnumerable<string> Slice(int start, int length);

    string Text { get; }
}
