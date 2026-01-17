using FluentValidation;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Users.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => new { x.Email, x.Login }).MustBeValid(o => UserAccountData.Create(o.Email, o.Login));
        RuleFor(x => x.Phone).MustBeValid(UserPhoneNumber.Create);
    }
}