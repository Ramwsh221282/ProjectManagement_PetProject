using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AssignMemberToTask;

public sealed class AssignMemberToTaskHandler(
    IProjectsRepository projects,
    ITransactionSource source,
    IUnitOfWork unitOfWork,
    IValidator<AssignMemberToTaskCommand> validator
)
{
    public async Task<Result<ProjectTaskAssignment>> Handle(
        AssignMemberToTaskCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<ProjectTaskAssignment>();

        await using ITransactionScope scope = await source.BeginTransactionScope(ct);
        var project = await projects.GetProject(command.ProjectId, withLock: true, ct: ct);
        if (project.IsFailure)
            return project.OnError;

        Result<ProjectMember> assigner = project.OnSuccess.FindMember(command.AssignerId);
        if (assigner.IsFailure)
            return assigner.OnError;

        if (!assigner.OnSuccess.IsOwning(project.OnSuccess))
            return Error.Conflict("Участник не является владельцем проекта.");

        Result<ProjectTask> task = project.OnSuccess.FindTask(command.TaskId);
        if (task.IsFailure)
            return Error.NotFound("Задача не найдена.");

        Result<ProjectMember> member = project.OnSuccess.FindMember(command.MemberId);
        if (member.IsFailure)
            return Error.NotFound("Участник не найден.");

        Result<ProjectTaskAssignment> assignment = project.OnSuccess.FormAssignment(
            task.OnSuccess,
            member.OnSuccess
        );
        if (assignment.IsFailure)
            return assignment.OnError;

        task.OnSuccess.AddAssignment(assignment.OnSuccess);
        member.OnSuccess.AssignTo(assignment.OnSuccess);

        if (await projects.Exists(assignment.OnSuccess, ct))
            return Error.Conflict("Участник уже записан на эту задачу.");

        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        Result commit = await scope.CommitAsync(ct);
        return commit.IsFailure ? commit.OnError : assignment.OnSuccess;
    }
}
