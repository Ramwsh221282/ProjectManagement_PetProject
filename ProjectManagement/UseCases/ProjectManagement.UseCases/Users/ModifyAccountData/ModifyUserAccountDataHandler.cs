using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Users.ModifyAccountData;

public sealed class ModifyUserAccountDataHandler(IUsersRepository users, IUnitOfWork unitOfWork)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task<Result<User, Error>> Handle(
        ModifyUserAccountDataCommand command, 
        CancellationToken ct = default)
    {
        if (!command.HasChanges) 
            return Failure<User, Error>(Error.Conflict("Нет изменений для обновления аккаунта пользователя"));
        
        Result<User, Nothing> user = await Users.GetUser(command.UserId, ct);
        if (user.IsFailure) 
            return Failure<User, Error>(Error.NotFound("Пользователь не найден."));
        
        Result<Unit, Error> update = await user.OnSuccess.UpdateUserAccountData(Users, command.Email, command.Login);
        if (update.IsFailure) return Failure<User, Error>(update.OnError);
        
        Result<Unit, Error> saving = await UnitOfWork.SaveChangesAsync(ct);
        return saving.IsFailure 
            ? Failure<User, Error>(saving.OnError) 
            : Success<User, Error>(user.OnSuccess);
    }
}