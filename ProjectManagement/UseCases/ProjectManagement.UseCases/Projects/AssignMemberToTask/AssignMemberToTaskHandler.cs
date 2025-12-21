using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;

namespace ProjectManagement.UseCases.Projects.AssignMemberToTask;

public sealed class AssignMemberToTaskHandler(IProjectsRepository projects, IUnitOfWork unitOfWork)
{
    public async Task<ProjectTaskAssignment> Handle(AssignMemberToTaskCommand command, CancellationToken ct = default)
    {
        Project? project = await projects.GetProject(command.ProjectId, ct: ct);
        if (project is null) throw new InvalidOperationException("Задача не найдена.");

        ProjectMember assigner = project.FindMember(command.AssignerId);
        if (!assigner.IsOwning(project)) throw new InvalidOperationException("Участник не является владельцем проекта.");
        
        ProjectTask task = project.FindTask(command.TaskId);
        ProjectMember member = project.FindMember(command.MemberId);

        ProjectTaskAssignment assignment = project.FormAssignment(task, member);
        await unitOfWork.SaveChangesAsync(ct);
        return assignment;
    }
}