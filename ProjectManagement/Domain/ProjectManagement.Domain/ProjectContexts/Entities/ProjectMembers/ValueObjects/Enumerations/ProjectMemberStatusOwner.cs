using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Обладатель проекта
/// </summary>
public sealed class ProjectMemberStatusOwner : Enumeration<ProjectMemberStatus>
{
    public ProjectMemberStatusOwner()
        : base(0, "Обладатель") { }
}
