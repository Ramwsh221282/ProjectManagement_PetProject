using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.Ports;

namespace ProjectManagement.UseCases.Projects.AddProjectTask;

public sealed class AddProjectTaskCommandHandler
{
    private readonly IProjectsRepository _repository;

    public AddProjectTaskCommandHandler(IProjectsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ProjectTask> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        ProjectTaskId id = ProjectTaskId.Create(Guid.NewGuid());
        ProjectTaskMembersLimit membersLimit = ProjectTaskMembersLimit.Create(request.MembersLimit);
        ProjectTaskStatusInfo status = new(
            ProjectTaskStatus.FromName(request.Status),
            ProjectTaskSchedule.Create(DateTime.UtcNow, request.FinishedAt)
        );
        
        Project? project = await _repository.GetProject(request.ProjectName, cancellationToken);
        
        ProjectTaskInfo info = ProjectTaskInfo.Create(request.Title, request.Description);
        ProjectTask task = new(id, membersLimit, status, info, project, []);
        project.AddTask(task);
        return task;
    }
}