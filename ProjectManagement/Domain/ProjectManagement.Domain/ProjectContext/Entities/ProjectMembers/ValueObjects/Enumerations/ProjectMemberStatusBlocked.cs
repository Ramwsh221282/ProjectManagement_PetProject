using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Заблокированный участник проекта
/// </summary>
public sealed class ProjectMemberStatusBlocked : Enumeration<ProjectMemberStatus>
{
    public ProjectMemberStatusBlocked(int value, string name)
        : base(3, "Заблокирован") { }
}
