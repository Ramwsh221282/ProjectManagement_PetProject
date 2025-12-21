namespace ProjectManagement.Domain.Contracts;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
}