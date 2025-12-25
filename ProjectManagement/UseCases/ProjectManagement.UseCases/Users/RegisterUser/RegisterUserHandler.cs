using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Users.RegisterUser;

public sealed class RegisterUserHandler
{
    private IUsersRepository Users { get; }
    private IUnitOfWork UnitOfWork { get; }

    public RegisterUserHandler(IUsersRepository users, IUnitOfWork unitOfWork)
    {
        Users = users;
        UnitOfWork = unitOfWork;
    }
    
    public async Task<Result<User, Error>> Handle(RegisterUserCommand command, CancellationToken ct = default)
    {
        Result<UserAccountData, Error> accountData = UserAccountData.Create(command.Email, command.Login);
        if (accountData.IsFailure) return Failure<User, Error>(accountData.OnError);
        
        Result<UserPhoneNumber, Error> phone = UserPhoneNumber.Create(command.Phone);
        if (phone.IsFailure) return Failure<User, Error>(phone.OnError);
        
        UserRegistrationApproval approval = await Users.CheckRegistrationApproval(
            accountData.OnSuccess.Email, 
            accountData.OnSuccess.Login, 
            phone.OnSuccess.Phone, ct);
        
        Result<User, Error> user = User.CreateNew(accountData.OnSuccess, phone.OnSuccess, approval);
        if (user.IsFailure) return Failure<User, Error>(user.OnError);
        
        await Users.Add(user.OnSuccess, ct);
        Result<Unit, Error> saving = await UnitOfWork.SaveChangesAsync(ct);
        return saving.IsFailure ? Failure<User, Error>(saving.OnError) : user;
    }
}