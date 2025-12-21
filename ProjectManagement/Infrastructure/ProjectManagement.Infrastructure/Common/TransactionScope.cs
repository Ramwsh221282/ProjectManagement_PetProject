using Microsoft.EntityFrameworkCore.Storage;
using ProjectManagement.Domain.Contracts;

namespace ProjectManagement.Infrastructure.Common;

public sealed class TransactionScope(IDbContextTransaction transaction) : ITransactionScope
{
    public void Dispose()
    {
        transaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        try
        {
            await transaction.CommitAsync(ct);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}