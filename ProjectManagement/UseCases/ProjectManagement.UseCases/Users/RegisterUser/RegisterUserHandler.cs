using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Users.RegisterUser;

public sealed class RegisterUserHandler
{
    private IUsersRepository Users { get; }
    private IUnitOfWork UnitOfWork { get; }
    private IValidator<RegisterUserCommand> Validator { get; }

    public RegisterUserHandler(
        IUsersRepository users,
        IUnitOfWork unitOfWork,
        IValidator<RegisterUserCommand> validator
    )
    {
        Users = users;
        UnitOfWork = unitOfWork;
        Validator = validator;
    }

    public async Task<Result<User>> Handle(
        RegisterUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await Validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<User>();

        Result<UserAccountData> accountData = UserAccountData.Create(command.Email, command.Login);
        Result<UserPhoneNumber> phone = UserPhoneNumber.Create(command.Phone);

        UserRegistrationApproval approval = await Users.CheckRegistrationApproval(
            accountData.OnSuccess.Email,
            accountData.OnSuccess.Login,
            phone.OnSuccess.Phone,
            ct
        );

        Result<User> user = User.CreateNew(accountData.OnSuccess, phone.OnSuccess, approval);
        if (user.IsFailure)
            return user.OnError;

        await Users.Add(user.OnSuccess, ct);
        Result saving = await UnitOfWork.SaveChangesAsync(ct);
        return saving.IsFailure ? saving.OnError : user;
    }
}
