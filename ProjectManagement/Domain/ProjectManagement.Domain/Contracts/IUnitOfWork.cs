namespace ProjectManagement.Domain.Contracts;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}