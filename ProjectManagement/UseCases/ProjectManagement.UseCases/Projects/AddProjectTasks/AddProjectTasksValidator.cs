using FluentValidation;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed class AddProjectTasksValidator : AbstractValidator<AddProjectTasksCommand>
{
    public AddProjectTasksValidator()
    {
        RuleFor(x => x.CreatorId).MustBeValid(UserId.Create);
        RuleFor(x => x.ProjectId).MustBeValid(ProjectId.Create);
        RuleFor(x => x.Tasks)
            .EachMustBeValid([
                t => ProjectTaskInfo.Create(t.Title, t.Description),
                t => ProjectTaskMembersLimit.Create(t.MembersLimit),
            ]);
    }
}
