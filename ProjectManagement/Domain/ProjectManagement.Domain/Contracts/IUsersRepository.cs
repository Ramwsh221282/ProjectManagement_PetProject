using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface IUsersRepository
{
    Task<Result<User, Nothing>> GetUser(Guid id, CancellationToken ct = default);
    Task<IEnumerable<User>> GetUsers(IEnumerable<Guid> ids, bool withLock = false, CancellationToken ct = default);
    Task<UserRegistrationApproval> CheckRegistrationApproval(
        string email, 
        string login, 
        string phone, 
        CancellationToken ct = default);
    Task Add(User user, CancellationToken ct = default);
    void Delete(User user);
}