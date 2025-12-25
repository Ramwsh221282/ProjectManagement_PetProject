using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.CloseProjectTask;

public sealed class CloseProjectTaskHandler(
    IProjectsRepository repository, 
    ITransactionSource transactionSource, 
    IUnitOfWork unitOfWork)
{
    public async Task<Result<ProjectTask, Error>> Handle(CloseProjectTaskCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);
        
        Result<Project, Nothing> project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure)
            return Failure<ProjectTask, Error>(Error.NotFound("Проект не найден."));
        
        Result<ProjectMember, Nothing> closer = project.OnSuccess.FindMember(command.CloserId);
        if (closer.IsFailure)
            return Failure<ProjectTask, Error>(Error.NotFound("Обладатель проекта не найден."));
        
        Result<ProjectTask, Nothing> task = project.OnSuccess.FindTask(command.TaskId);
        if (task.IsFailure)
            return Failure<ProjectTask, Error>(Error.NotFound("Задача не найдена."));
        
        Result<Unit, Error> closing = project.OnSuccess.CloseTask(task.OnSuccess);
        if (closing.IsFailure)
            return Failure<ProjectTask, Error>(closing.OnError);
        
        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<ProjectTask, Error>(saving.OnError);
        
        Result<Unit, Error> commit = await scope.CommitAsync(ct); 
        
        return commit.IsFailure 
            ? Failure<ProjectTask, Error>(commit.OnError) 
            : Success<ProjectTask, Error>(task.OnSuccess);
    }
}