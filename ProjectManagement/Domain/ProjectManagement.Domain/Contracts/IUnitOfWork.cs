using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface IUnitOfWork
{
    Task<Result<Unit, Error>> SaveChangesAsync(CancellationToken ct = default);
}