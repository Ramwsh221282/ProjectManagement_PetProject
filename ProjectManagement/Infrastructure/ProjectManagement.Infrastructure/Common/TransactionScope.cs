using Microsoft.EntityFrameworkCore.Storage;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Infrastructure.Common;

public sealed class TransactionScope(IDbContextTransaction transaction) : ITransactionScope
{
    public void Dispose() => transaction.Dispose();

    public ValueTask DisposeAsync() => transaction.DisposeAsync();

    public async Task<Result<Unit>> CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await transaction.CommitAsync(ct);
            return Unit.Value;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            return Error.InternalError("Ошибка транзакции");
        }
    }
}
