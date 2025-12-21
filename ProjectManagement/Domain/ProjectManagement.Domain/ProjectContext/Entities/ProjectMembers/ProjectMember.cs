using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;

/// <summary>
/// Участник проекта
/// </summary>
public sealed class ProjectMember
{
    private ProjectMember() { } // ef core

    /// <summary>
    /// Задачи, на которые участник записался
    /// </summary>
    private List<ProjectTaskAssignment> _assignments { get; set; } = [];

    /// <summary>
    /// Проект в котором состоит участник
    /// </summary>
    public Project? Project { get; private set; }

    /// <summary>
    /// Ид проекта
    /// </summary>
    public ProjectId? ProjectId { get; private set; }

    /// <summary>
    /// Идентификатор участника проекта
    /// </summary>
    public ProjectMemberId MemberId { get; private set; }

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

    public bool ExistsIn(IEnumerable<ProjectMember> members)
    {
        return members.Any(m => m.MemberId == MemberId);
    }

    public bool IsOwning(Project project)
    {
        return MemberId.Value == project.Ownership.OwnerId.Id;
    }

    public void JoinTo(Project project)
    {
        Project = project;
        ProjectId = project.Id;
    }
    
    public void AssignTo(ProjectTaskAssignment assignment)
    {
        if (_assignments.Any(a => a.MemberId == assignment.MemberId && a.TaskId == assignment.TaskId))
            throw new InvalidOperationException("Участник уже назначен на эту задачу.");
        _assignments.Add(assignment);
    }
    
    public static ProjectMember CreateNewContributor(ProjectMemberId id, ProjectMemberLogin login)
    {
        ProjectMemberStatus status = new ProjectMemberStatusContributor();
        return new ProjectMember()
        {
            MemberId = id,
            _assignments = [],
            Login = login,
            Status = status,
            Project = null,
            ProjectId = null,
        };
    }

    public static ProjectMember CreateOwner(ProjectMemberId id, ProjectMemberLogin login)
    {
        ProjectMemberStatus status = new ProjectMemberStatusOwner();
        return new ProjectMember()
        {
            MemberId = id,
            _assignments = [],
            Login = login,
            Status = status,
            Project = null,
            ProjectId = null,
        };
    }
}
