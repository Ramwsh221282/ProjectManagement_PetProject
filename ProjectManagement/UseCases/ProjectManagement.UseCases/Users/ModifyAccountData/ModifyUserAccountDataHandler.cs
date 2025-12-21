using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.UseCases.Users.ModifyAccountData;

public sealed class ModifyUserAccountDataHandler(IUsersRepository users, IUnitOfWork unitOfWork)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;

    public async Task<User> Handle(ModifyUserAccountDataCommand command, CancellationToken ct = default)
    {
        if (!command.HasChanges) throw new InvalidOperationException("Нет изменений для обновления аккаунта пользователя");
        
        User? user = await Users.GetUser(command.UserId, ct);
        if (user is null) throw new InvalidOperationException("Пользователь не найден.");
        
        await user.UpdateUserAccountData(users, command.Email, command.Login);
        await UnitOfWork.SaveChangesAsync(ct);
        return user;
    }
}