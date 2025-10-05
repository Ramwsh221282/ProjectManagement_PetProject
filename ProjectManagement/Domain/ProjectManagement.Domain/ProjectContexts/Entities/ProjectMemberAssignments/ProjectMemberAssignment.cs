using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMemberAssignments.ValueObjects;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMemberAssignments;

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
