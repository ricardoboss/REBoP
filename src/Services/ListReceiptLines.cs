using System.Collections;
using System.Diagnostics;

namespace REBoP.Services;

[DebuggerDisplay("[Line Count = {Count}] {Text}")]
public class ListReceiptLines(IReadOnlyList<string> lines) : IReceiptLines
{
    public IEnumerator<string> GetEnumerator() => lines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => lines.Count;

    public string this[int index] => lines[index];

    public IEnumerable<string> Slice(int start, int length) => lines.Skip(start).Take(length);

    public string Text => string.Join(Environment.NewLine, this);
}
