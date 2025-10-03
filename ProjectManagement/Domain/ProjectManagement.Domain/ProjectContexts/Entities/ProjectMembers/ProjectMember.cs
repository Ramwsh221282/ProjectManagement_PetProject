using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers;

public sealed class ProjectMember
{
    public ProjectMemberId Id { get; }
    public ProjectMemberLogin Login { get; private set; }
    public ProjectMemberStatus Status { get; private set; }

    public ProjectMember(ProjectMemberId id, ProjectMemberLogin login, ProjectMemberStatus status)
    {
        Id = id;
        Login = login;
        Status = status;
    }
}