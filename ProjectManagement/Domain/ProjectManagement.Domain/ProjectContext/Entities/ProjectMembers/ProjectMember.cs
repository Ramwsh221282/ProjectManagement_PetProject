using ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;

/// <summary>
/// Участник проекта
/// </summary>
public sealed class ProjectMember
{
    /// <summary>
    /// Задачи, на которые участник записался
    /// </summary>
    private readonly List<ProjectTaskAssignment> _assignments;

    /// <summary>
    /// Проект в котором состоит участник
    /// </summary>
    public Project Project { get; }

    /// <summary>
    /// Ид проекта
    /// </summary>
    public ProjectId ProjectId { get; }

    /// <summary>
    /// Идентификатор участника проекта
    /// </summary>
    public ProjectMemberId MemberId { get; }

    /// <summary>
    /// Логин участника проекта
    /// </summary>
    public ProjectMemberLogin Login { get; private set; }

    /// <summary>
    /// Статус участника проекта
    /// </summary>
    public ProjectMemberStatus Status { get; private set; }

    public IReadOnlyList<ProjectTaskAssignment> Assignments => _assignments;

    public ProjectMember(
        ProjectMemberId id,
        ProjectMemberLogin login,
        ProjectMemberStatus status,
        Project project,
        IEnumerable<ProjectTaskAssignment> assignments
    )
    {
        MemberId = id;
        Project = project;
        ProjectId = project.Id;
        Login = login;
        Status = status;
        Project = project;
        _assignments = [.. assignments];
    }
}
