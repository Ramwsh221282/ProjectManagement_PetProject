using FluentValidation;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AddProjectMembers;

public sealed class AddProjectMembersValidator : AbstractValidator<AddProjectMembersCommand>
{
    public AddProjectMembersValidator()
    {
        RuleFor(x => x.CreatorId).MustBeValid(UserId.Create);
        RuleFor(x => x.ProjectId).MustBeValid(ProjectId.Create);
        RuleFor(x => x.MemberIds).EachMustBeValid([i => ProjectMemberId.Create(i)]);
    }
}
