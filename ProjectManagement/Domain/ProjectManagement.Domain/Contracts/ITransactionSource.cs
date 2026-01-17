namespace ProjectManagement.Domain.Contracts;

public interface ITransactionSource
{
    Task<ITransactionScope> BeginTransactionScope(CancellationToken ct = default);
}