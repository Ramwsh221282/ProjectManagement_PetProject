using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Infrastructure.Common;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task<Result<Unit, Error>> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await context.SaveChangesAsync(ct);
            return Success<Unit, Error>(Unit.Value);
        }
        catch (Exception)
        {
            return Failure<Unit, Error>(Error.InternalError("Ошибка сохранения изменений"));
        }
    }
}