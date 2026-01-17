using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Infrastructure.Common;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task<Result> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await context.SaveChangesAsync(ct);
            return Success();
        }
        catch (Exception)
        {
            return Error.InternalError("Ошибка сохранения изменений");
        }
    }
}
