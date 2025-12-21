using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Infrastructure.Common;

namespace ProjectManagement.Infrastructure.UserContext;

public sealed class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<User?> GetUser(Guid id, CancellationToken ct = default)
    {
        UserId userId = UserId.Create(id);
        User? user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken: ct);
        return user;
    }

    public async Task<IEnumerable<User>> GetUsers(IEnumerable<Guid> ids, bool withLock = false, CancellationToken ct = default)
    {
        string parameter = "'{" + string.Join(",", ids.Select(i => $"'{i}'")) + "}'";

        FormattableString sql = withLock switch
        {
            true => $"SELECT * FROM USERS WHERE id = ANY({parameter}) FOR UPDATE",
            false => $"SELECT * FROM USERS WHERE id = ANY({parameter})"
        };

        return await context.Users.FromSqlInterpolated(sql).ToListAsync(ct);
    }

    public async Task<User?> GetUserDetached(Guid id, CancellationToken ct = default)
    {
        UserId userId = UserId.Create(id);
        
        User? user = await context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken: ct);

        if (user is null) return user;

        EntityEntry<User> entry = context.ChangeTracker
            .Entries<User>()
            .First(u => u.Entity.UserId == user.UserId);
        
        entry.State = EntityState.Detached;
        
        return user;
    }
    
    public async Task<UserRegistrationApproval> CheckRegistrationApproval(
        string email, 
        string login, 
        string phone, 
        CancellationToken ct = default)
    {
        FormattableString sql = $@"
            SELECT
            EXISTS(SELECT 1 FROM users WHERE email = {email}) as email_exists,
            EXISTS(SELECT 1 FROM users WHERE login = {login}) as login_exists,
            EXISTS(SELECT 1 FROM users WHERE phone_number = {phone}) as phone_exists;
         ";
        
        (bool emailExists, bool loginExists, bool phoneExists) = await context.Database
            .SqlQuery<(bool, bool, bool)>(sql).FirstAsync(ct);
        
        bool isEmailUnique = !emailExists;
        bool isLoginUnique = !loginExists;
        bool isPhoneUnique = !phoneExists;
        
        return new UserRegistrationApproval(
            HasUniqueEmail: isEmailUnique, 
            HasUniqueLogin: isLoginUnique, 
            HasUniquePhone: isPhoneUnique);
    }
    
    public async Task Add(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, cancellationToken: ct);
    }

    public void Delete(User user)
    {
        context.Users.Remove(user);
    }
}