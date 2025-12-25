using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.AssignMemberToTask;

public sealed class AssignMemberToTaskHandler(
    IProjectsRepository projects,
    ITransactionSource source,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<ProjectTaskAssignment, Error>> Handle(AssignMemberToTaskCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await source.BeginTransactionScope(ct);
        var project = await projects.GetProject(command.ProjectId, withLock: true, ct: ct);
        if (project.IsFailure)
            return Failure<ProjectTaskAssignment, Error>(Error.NotFound("Проект не найден."));

        Result<ProjectMember, Nothing> assigner = project.OnSuccess.FindMember(command.AssignerId);
        if (assigner.IsFailure)
            return Failure<ProjectTaskAssignment, Error>(Error.NotFound("Обладатель проекта не найден."));
        
        if (!assigner.OnSuccess.IsOwning(project.OnSuccess)) 
            return Failure<ProjectTaskAssignment, Error>(Error.Conflict("Участник не является владельцем проекта."));
        
        Result<ProjectTask, Nothing> task = project.OnSuccess.FindTask(command.TaskId);
        if (task.IsFailure)
            return Failure<ProjectTaskAssignment, Error>(Error.NotFound("Задача не найдена."));
        
        Result<ProjectMember, Nothing> member = project.OnSuccess.FindMember(command.MemberId);
        if (member.IsFailure)
            return Failure<ProjectTaskAssignment, Error>(Error.NotFound("Участник не найден."));
        
        Result<ProjectTaskAssignment, Error> assignment = project.OnSuccess.FormAssignment(task.OnSuccess, member.OnSuccess);
        if (assignment.IsFailure)
            return Failure<ProjectTaskAssignment, Error>(assignment.OnError);
            
        task.OnSuccess.AddAssignment(assignment.OnSuccess);
        member.OnSuccess.AssignTo(assignment.OnSuccess);
        
        if (await projects.Exists(assignment.OnSuccess, ct))
            return Failure<ProjectTaskAssignment, Error>(Error.Conflict("Участник уже записан на эту задачу."));

        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<ProjectTaskAssignment, Error>(saving.OnError);
        
        Result<Unit, Error> commit = await scope.CommitAsync(ct);
        return commit.IsFailure 
            ? Failure<ProjectTaskAssignment, Error>(commit.OnError) 
            : Success<ProjectTaskAssignment, Error>(assignment.OnSuccess);
    }
}