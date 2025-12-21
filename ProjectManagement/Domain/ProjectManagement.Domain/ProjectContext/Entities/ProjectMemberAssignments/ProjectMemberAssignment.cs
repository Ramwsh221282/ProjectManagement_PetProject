using ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments;

/// <summary>
/// Назначение участника проекта к задаче
/// </summary>
public sealed class ProjectTaskAssignment
{
    private ProjectTaskAssignment() { } // ef core

    /// <summary>
    /// Задача
    /// </summary>
    public ProjectTask Task { get; }

    /// <summary>
    /// Ид задачи
    /// </summary>
    public ProjectTaskId TaskId { get; }

    /// <summary>
    /// Участник
    /// </summary>
    public ProjectMember Member { get; }

    /// <summary>
    /// Ид участника
    /// </summary>
    public ProjectMemberId MemberId { get; }

    /// <summary>
    /// Дата назначения участника в задачу
    /// </summary>
    public ProjectTaskAssignmentDate AssignmentDate { get; }

    public ProjectTaskAssignment(ProjectTask task, ProjectMember member, ProjectTaskAssignmentDate assignmentDate)
    {
        Task = task;
        TaskId = task.Id;
        Member = member;
        MemberId = member.MemberId;
        AssignmentDate = assignmentDate;
    }
    
    /// <summary>
    /// Создать назначение участника к задаче
    /// </summary>
    /// <param name="task">Задача</param>
    /// <param name="member">Участник</param>
    /// <returns>Назначение участника к задаче</returns>
    public static ProjectTaskAssignment FormAssignmentByCurrentDate(ProjectTask task, ProjectMember member)
    {
        ProjectTaskAssignmentDate date = ProjectTaskAssignmentDate.Current();
        ProjectTaskAssignment assignment = new ProjectTaskAssignment(task, member, date);
        task.AddAssignment(assignment);
        member.AssignTo(assignment);
        return assignment;
    }
}
