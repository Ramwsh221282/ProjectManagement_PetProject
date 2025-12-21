using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.UseCases.Users.RemoveUserProfile;

public sealed class RemoveUserProfileHandler(IUsersRepository users, IUnitOfWork unitOfWork)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task Handle(
        RemoveUserCommand command, 
        CancellationToken ct = default)
    {
        // Здесь User приходит с state Attached (привязан к контексту)
        User? user = await Users.GetUser(command.UserId, ct);
        if (user is null) 
            throw new InvalidOperationException("Пользователь не найден.");

        // Здесь User переходит в state Deleted (будет удален)
        Users.Delete(user);
        
        // Здесь будет сгенерирован и отправлен SQL запрос с DELETE
        await UnitOfWork.SaveChangesAsync(ct);
    }
}