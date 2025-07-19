using System.Diagnostics;

namespace REBoP.Models;

[DebuggerDisplay("{Alias}={Bracket}")]
public class TaxBracketAlias
{
    public required Guid Id { get; init; }

    public required TaxBracket Bracket { get; init; }

    public required string Alias { get; init; }
}
