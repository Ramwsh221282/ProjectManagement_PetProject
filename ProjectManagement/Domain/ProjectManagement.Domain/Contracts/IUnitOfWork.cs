using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken ct = default);
}
