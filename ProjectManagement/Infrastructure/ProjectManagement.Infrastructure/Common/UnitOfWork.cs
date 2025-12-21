using ProjectManagement.Domain.Contracts;

namespace ProjectManagement.Infrastructure.Common;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }
}