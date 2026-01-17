using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Заблокированный участник проекта
/// </summary>
public sealed class ProjectMemberStatusBlocked : ProjectMemberStatus
{
    public ProjectMemberStatusBlocked(int value, string name)
        : base(3, "Заблокирован") { }
}
