using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task<Result<Unit, Error>> CommitAsync(CancellationToken ct = default);
}