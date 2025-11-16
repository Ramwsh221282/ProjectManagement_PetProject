namespace ProjectManagement.Domain.UserContext.Ports;

public interface IUsersRepository
{
    Task<User?> Get(Guid id, CancellationToken ct);
}