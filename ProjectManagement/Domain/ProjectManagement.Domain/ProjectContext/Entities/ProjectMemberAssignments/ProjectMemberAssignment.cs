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

    public ProjectTaskAssignment(ProjectTask task, ProjectMember member)
    {
        Task = task;
        TaskId = task.Id;
        Member = member;
        MemberId = member.MemberId;
    }
}
