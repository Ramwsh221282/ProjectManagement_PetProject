using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Users.RemoveUserProfile;

public sealed class RemoveUserProfileHandler(IUsersRepository users, IUnitOfWork unitOfWork)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task<Result<User, Error>> Handle(RemoveUserCommand command, CancellationToken ct = default)
    {
        Result<User, Nothing> user = await Users.GetUser(command.UserId, ct);
        if (user.IsFailure) return Failure<User, Error>(Error.NotFound("Пользователь не найден."));
        
        Users.Delete(user.OnSuccess);
        Result<Unit, Error> saving = await UnitOfWork.SaveChangesAsync(ct);
        return saving.IsFailure ? Failure<User, Error>(saving.OnError) : Success<User, Error>(user.OnSuccess);
    }
}