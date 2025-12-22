using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Domain.Contracts;

public interface IUsersRepository
{
    Task<User?> GetUser(Guid id, CancellationToken ct = default);
    Task<IEnumerable<User>> GetUsers(IEnumerable<Guid> ids, bool withLock = false, CancellationToken ct = default);
    Task<User?> GetUserDetached(Guid id, CancellationToken ct = default);
    Task<UserRegistrationApproval> CheckRegistrationApproval(
        string email, 
        string login, 
        string phone, 
        CancellationToken ct = default);
    Task Add(User user, CancellationToken ct = default);
    void Delete(User user);
    Task Update(User user, CancellationToken ct = default);
}