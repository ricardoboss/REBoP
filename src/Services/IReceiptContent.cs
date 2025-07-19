namespace REBoP.Services;

public interface IReceiptContent
{
    IPdfSource Source { get; }

    IReceiptLines Lines { get; }
}

public static class ReceiptContentExtensions
{
    public static IEnumerable<string> FindLines(this IReceiptContent content, Func<string, int, bool> startPredicate,
        Func<string, int, bool> endPredicate)
    {
        var maybeStartLineIdx = FindLineIdx(content, startPredicate);
        var maybeEndLineIdx = FindLineIdx(content, endPredicate, inReverse: true);

        if (maybeStartLineIdx is not { } startLineIdx || maybeEndLineIdx is not { } endLineIdx)
            return [];

        // include last line that matched
        return content.Lines[startLineIdx..(endLineIdx + 1)];
    }

    public static string? FindLine(this IReceiptContent content, Func<string, int, bool> predicate)
    {
        return content.Lines.Index().LastOrDefault(t => predicate(t.Item, t.Index)).Item;
    }

    public static int? FindLineIdx(this IReceiptContent content, Func<string, int, bool> predicate,
        bool inReverse = false)
    {
        var indexed = content.Lines.Index();
        if (inReverse)
            indexed = indexed.Reverse();

        var (idx, line) = indexed.FirstOrDefault(t => predicate(t.Item, t.Index));
        if (line is null)
            return null;

        return idx;
    }
}
