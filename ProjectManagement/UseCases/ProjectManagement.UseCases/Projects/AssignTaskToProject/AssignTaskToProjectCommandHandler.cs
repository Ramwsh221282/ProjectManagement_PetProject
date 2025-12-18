using System.Net;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Ports;

namespace ProjectManagement.UseCases.Projects.AssignTaskToProject;

public sealed class AssignTaskToProjectCommandHandler
{
    private readonly IProjectsRepository _repository;

    public AssignTaskToProjectCommandHandler(IProjectsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ProjectTaskAssignment> Handle(AssignTaskToProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _repository.GetProject(request.Name, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project was not found.");    

        ProjectTask? task = project.Tasks.FirstOrDefault(t => t.Id.Value == request.TaskId);
        if (task == null)
            throw new InvalidOperationException("Task was not found.");
            

        ProjectMember? projectMember = project.Members.FirstOrDefault(m => m.MemberId.Value == request.MemberId);
        if (projectMember == null)
            throw new InvalidOperationException("Project member was not found.");

        ProjectTaskAssignment assignment = new ProjectTaskAssignment(task, projectMember);
        task.AddAssignment(assignment);
        return assignment;
    }
}