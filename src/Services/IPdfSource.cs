using JetBrains.Annotations;

namespace REBoP.Services;

public interface IPdfSource
{
    [MustDisposeResource]
    FileStream OpenRead();
}
