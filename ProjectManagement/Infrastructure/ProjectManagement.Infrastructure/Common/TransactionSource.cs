using Microsoft.EntityFrameworkCore.Storage;
using ProjectManagement.Domain.Contracts;

namespace ProjectManagement.Infrastructure.Common;

public sealed class TransactionSource(ApplicationDbContext context) : ITransactionSource
{
    public async Task<ITransactionScope> BeginTransactionScope(CancellationToken ct = default)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(ct);
        return new TransactionScope(transaction);
    }
}