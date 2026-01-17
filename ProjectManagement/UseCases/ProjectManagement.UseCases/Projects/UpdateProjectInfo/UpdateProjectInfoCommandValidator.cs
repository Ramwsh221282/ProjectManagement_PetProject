using FluentValidation;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.UpdateProjectInfo;

public sealed class UpdateProjectInfoCommandValidator : AbstractValidator<UpdateProjectInfoCommand>
{
    public UpdateProjectInfoCommandValidator()
    {
        RuleFor(x => x.CreatorId).MustBeValid(UserId.Create);
        RuleFor(x => x.ProjectId).MustBeValid(ProjectId.Create);
        RuleFor(x => x.NewName).MustBeValidIfProvided(ProjectName.Create);
        RuleFor(x => x.NewDescription).MustBeValidIfProvided(ProjectDescription.Create);
    }
}