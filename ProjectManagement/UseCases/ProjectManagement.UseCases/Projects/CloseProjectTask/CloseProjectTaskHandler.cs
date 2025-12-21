using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;

namespace ProjectManagement.UseCases.Projects.CloseProjectTask;

public sealed class CloseProjectTaskHandler(
    IProjectsRepository repository, 
    ITransactionSource transactionSource, 
    IUnitOfWork unitOfWork)
{
    public async Task<ProjectTask> Handle(CloseProjectTaskCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);
        
        Project? project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project is null) throw new InvalidOperationException("Проект не найден.");
        
        ProjectMember closer = project.FindMember(command.CloserId);
        if (!closer.IsOwning(project)) throw new InvalidOperationException("Участник не является владельцем проекта.");
        
        ProjectTask task = project.FindTask(command.TaskId);
        project.CloseTask(task);
        
        await unitOfWork.SaveChangesAsync(ct);
        await scope.CommitAsync(ct); 
        
        return task;
    }
}