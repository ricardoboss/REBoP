using System.Runtime.CompilerServices;

namespace REBoP.Services;

/// <summary>
/// Lists all .pdf files (recursively) in the given <paramref name="dir"/>.
/// </summary>
/// <param name="dir">The dir to look for files in.</param>
public class DirectoryPdfProvider(DirectoryInfo dir) : IPdfProvider
{
    /// <inheritdoc />
    public async IAsyncEnumerable<IPdfSource> GetPdfsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var file in dir.EnumerateFiles("*.pdf", SearchOption.AllDirectories))
            yield return new FilesystemPdfSource(file);
    }
}
