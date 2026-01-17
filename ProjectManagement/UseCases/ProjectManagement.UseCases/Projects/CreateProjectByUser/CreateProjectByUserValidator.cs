using FluentValidation;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.CreateProjectByUser;

public sealed class CreateProjectByUserValidator : AbstractValidator<CreateProjectByUserCommand>
{
    public CreateProjectByUserValidator()
    {
        RuleFor(x => x.UserId).MustBeValid(UserId.Create);
        RuleFor(x => x.ProjectName).MustBeValid(ProjectName.Create);
        RuleFor(x => x.ProjectDescription).MustBeValid(ProjectName.Create);
    }
}