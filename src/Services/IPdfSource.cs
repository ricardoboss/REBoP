using JetBrains.Annotations;

namespace REBoP.Services;

public interface IPdfSource
{
    [MustDisposeResource]
    Stream OpenRead();
}
