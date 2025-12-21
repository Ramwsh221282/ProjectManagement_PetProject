using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.UseCases.Users.RemoveUserProfile;

public sealed class RemoveUserProfileHandler(IUsersRepository users, IUnitOfWork unitOfWork)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task<User> Handle(
        RemoveUserCommand command, 
        CancellationToken ct = default)
    {
        User? user = await Users.GetUser(command.UserId, ct);
        if (user is null) 
            throw new InvalidOperationException("Пользователь не найден.");
        
        Users.Delete(user);
        
        await UnitOfWork.SaveChangesAsync(ct);
        return user;
    }
}