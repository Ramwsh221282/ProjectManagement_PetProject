using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task<Result<Unit>> CommitAsync(CancellationToken ct = default);
}
