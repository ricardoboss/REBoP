using REBoP.Models;

namespace REBoP.Services;

public interface IReceiptLineInterpreter
{
    Task<Receipt> ProcessAsync(IReceiptContent content, CancellationToken cancellationToken = default);
}
