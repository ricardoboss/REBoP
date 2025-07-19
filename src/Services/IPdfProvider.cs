namespace REBoP.Services;

public interface IPdfProvider
{
    IAsyncEnumerable<IPdfSource> GetPdfsAsync(CancellationToken cancellationToken = default);
}
