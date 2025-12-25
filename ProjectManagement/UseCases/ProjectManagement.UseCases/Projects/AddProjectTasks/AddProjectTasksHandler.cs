using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed class AddProjectTasksHandler(
    IProjectsRepository repository,
    ITransactionSource transactionSource,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<IEnumerable<ProjectTask>, Error>> Handle(AddProjectTasksCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct); 
        
        Result<Project, Nothing> project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure) 
            return Failure<IEnumerable<ProjectTask>, Error>(Error.NotFound("Проект не найден."));
        
        Result<ProjectMember, Nothing> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure) 
            return Failure<IEnumerable<ProjectTask>, Error>(Error.NotFound("Участник не найден."));
        
        
        if (!creator.OnSuccess.IsOwning(project.OnSuccess)) 
            return Failure<IEnumerable<ProjectTask>, Error>(Error.Conflict("Участник не является владельцем проекта."));
        
        Result<ProjectTask[], Error> tasks = ConvertProjectTasksFromDto(command.Tasks);
        if (tasks.IsFailure) 
            return Failure<IEnumerable<ProjectTask>, Error>(tasks.OnError);
        
        project.OnSuccess.AddTasks(tasks.OnSuccess);
        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<IEnumerable<ProjectTask>, Error>(saving.OnError);
        
        Result<Unit, Error> commit = await scope.CommitAsync(ct);
        return commit.IsFailure 
            ? Failure<IEnumerable<ProjectTask>, Error>(commit.OnError)
            : Success<IEnumerable<ProjectTask>, Error>(tasks.OnSuccess);
    }

    private Result<ProjectTask[], Error> ConvertProjectTasksFromDto(IEnumerable<AddProjectTaskDto> tasks)
    {
        List<ProjectTask> results = [];
        DateTime creationDate = DateTime.UtcNow;
        foreach (AddProjectTaskDto task in tasks)
        {
            Result<ProjectTaskMembersLimit, Error> limit = ProjectTaskMembersLimit.Create(task.MembersLimit);
            if (limit.IsFailure) return Failure<ProjectTask[], Error>(limit.OnError);
            Result<ProjectTaskInfo, Error> info = ProjectTaskInfo.Create(task.Title, task.Description);
            if (info.IsFailure) return Failure<ProjectTask[], Error>(info.OnError);
            Result<ProjectTaskSchedule, Error> schedule = ProjectTaskSchedule.Create(creationDate, task.CloseDate);
            if (schedule.IsFailure) return Failure<ProjectTask[], Error>(schedule.OnError);
            results.Add(ProjectTask.CreateNew(limit.OnSuccess, info.OnSuccess, schedule.OnSuccess));
        }
        return Success<ProjectTask[], Error>(results.ToArray());
    }
}