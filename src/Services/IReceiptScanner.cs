namespace REBoP.Services;

public interface IReceiptScanner
{
    Task<IReceiptContent> ScanAsync(IPdfSource pdf, ScanOptions? options = null, CancellationToken cancellationToken = default);
}
