using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Users.ModifyAccountData;

public sealed class ModifyUserAccountDataHandler(
    IUsersRepository users,
    IUnitOfWork unitOfWork,
    IValidator<ModifyUserAccountDataCommand> validator
)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;
    private IValidator<ModifyUserAccountDataCommand> Validator { get; } = validator;

    public async Task<Result<User>> Handle(
        ModifyUserAccountDataCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await Validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<User>();

        if (!command.HasChanges)
            return Error.Conflict("Нет изменений для обновления аккаунта пользователя");

        Result<User> user = await Users.GetUser(command.UserId, ct);
        if (user.IsFailure)
            return user.OnError;

        Result<Unit> update = await user.OnSuccess.UpdateUserAccountData(
            Users,
            command.Email,
            command.Login
        );
        if (update.IsFailure)
            return update.OnError;

        Result saving = await UnitOfWork.SaveChangesAsync(ct);
        return saving.IsFailure ? update.OnError : user.OnSuccess;
    }
}
