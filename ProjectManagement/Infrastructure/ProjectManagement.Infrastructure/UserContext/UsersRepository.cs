using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.Infrastructure.Common;

namespace ProjectManagement.Infrastructure.UserContext;

public sealed class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<Result<User, Nothing>> GetUser(Guid id, CancellationToken ct = default)
    {
        Result<UserId, Error> userId = UserId.Create(id);
        if (userId.IsFailure) return Failure<User, Nothing>(new Nothing());
        
        User? user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId.OnSuccess, cancellationToken: ct);
        return user is null ? Failure<User, Nothing>(new Nothing()) : Success<User, Nothing>(user);
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
    
    public async Task<UserRegistrationApproval> CheckRegistrationApproval(
        string email, 
        string login, 
        string phone, 
        CancellationToken ct = default)
    {
        var emailParam = new NpgsqlParameter("@email", email);
        var loginParam = new NpgsqlParameter("@login", login);
        var phoneParam = new NpgsqlParameter("@phone", phone);
        
        RegistrationCheckResult result = await context.Database
            .SqlQueryRaw<RegistrationCheckResult>(
                @"
            SELECT
            EXISTS(SELECT 1 FROM users WHERE users.email = @email) as ""EmailExists"",
            EXISTS(SELECT 1 FROM users WHERE users.login = @login) as ""LoginExists"",
            EXISTS(SELECT 1 FROM users WHERE users.phone_number = @phone) as ""PhoneExists""    
         ", emailParam, loginParam, phoneParam
            ).FirstAsync(ct);
        
        bool isEmailUnique = !result.EmailExists;
        bool isLoginUnique = !result.LoginExists;
        bool isPhoneUnique = !result.PhoneExists;
        
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

    private class RegistrationCheckResult
    {
        public bool EmailExists { get; set; }
        public bool LoginExists { get; set; }
        public bool PhoneExists { get; set; }
    }
}