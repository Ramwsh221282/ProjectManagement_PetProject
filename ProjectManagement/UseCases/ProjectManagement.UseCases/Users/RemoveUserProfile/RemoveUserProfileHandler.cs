using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Users.RemoveUserProfile;

public sealed class RemoveUserProfileHandler(
    IUsersRepository users,
    IUnitOfWork unitOfWork,
    IValidator<RemoveUserCommand> validator
)
{
    private IUsersRepository Users { get; } = users;
    private IUnitOfWork UnitOfWork { get; } = unitOfWork;
    private IValidator<RemoveUserCommand> Validator { get; } = validator;

    public async Task<Result<User>> Handle(
        RemoveUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await Validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<User>();

        Result<User> user = await Users.GetUser(command.UserId, ct);
        if (user.IsFailure)
            return user.OnError;

        Users.Delete(user.OnSuccess);
        Result saving = await UnitOfWork.SaveChangesAsync(ct);

        return saving.IsFailure ? saving.OnError : user.OnSuccess;
    }
}
