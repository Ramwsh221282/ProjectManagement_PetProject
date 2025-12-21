using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed class AddProjectTasksCommandHandler(
    IProjectsRepository repository,
    ITransactionSource transactionSource,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<ProjectTask>> Handle(
        AddProjectTasksCommand command, 
        CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct); 
        
        Project? project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project is null) throw new InvalidOperationException("Проект не найден.");
        
        ProjectMember creator = project.FindMember(command.CreatorId);
        if (!creator.IsOwning(project)) throw new InvalidOperationException("Участник не является владельцем проекта.");
        
        ProjectTask[] tasks = ConvertProjectTasksFromDto(command.Tasks);
        project.AddTasks(tasks);
        
        await unitOfWork.SaveChangesAsync(ct);
        await scope.CommitAsync(ct);
        return tasks;
    }

    private ProjectTask[] ConvertProjectTasksFromDto(IEnumerable<AddProjectTaskDto> tasks)
    {
        DateTime creationDate = DateTime.UtcNow;
        return tasks.Select(t => ProjectTask.CreateNew(
                ProjectTaskMembersLimit.Create(t.MembersLimit),
                ProjectTaskInfo.Create(t.Title, t.Description),
                ProjectTaskSchedule.Create(creationDate, t.CloseDate)
            ))
            .ToArray();
    }
}