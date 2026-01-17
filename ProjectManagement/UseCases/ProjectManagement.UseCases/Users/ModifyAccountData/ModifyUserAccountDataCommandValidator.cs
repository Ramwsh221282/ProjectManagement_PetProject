using FluentValidation;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Users.ModifyAccountData;

public sealed class ModifyUserAccountDataCommandValidator
    : AbstractValidator<ModifyUserAccountDataCommand>
{
    public ModifyUserAccountDataCommandValidator()
    {
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
        RuleFor(x => new { x.Email, x.Login })
            .MustBeValidIfProvided(o => UserAccountData.Create(o.Email, o.Login));
    }
}
