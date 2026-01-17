using FluentValidation;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AssignMemberToTask;

public sealed class AssignMemberToTaskCommandValidator : AbstractValidator<AssignMemberToTaskCommand>
{
    public AssignMemberToTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).MustBeValid(ProjectId.Create);
        RuleFor(x => x.TaskId).MustBeValid(ProjectTaskId.Create);
        RuleFor(x => x.AssignerId).MustBeValid(UserId.Create);
    }
}