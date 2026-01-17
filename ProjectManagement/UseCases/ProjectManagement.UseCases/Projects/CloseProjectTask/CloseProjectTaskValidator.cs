using FluentValidation;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.CloseProjectTask;

public sealed class CloseProjectTaskValidator : AbstractValidator<CloseProjectTaskCommand>
{
    public CloseProjectTaskValidator()
    {
        RuleFor(x => x.CloserId).MustBeValid(UserId.Create);
        RuleFor(x => x.ProjectId).MustBeValid(ProjectId.Create);
        RuleFor(x => x.TaskId).MustBeValid(ProjectTaskId.Create);
    }
}