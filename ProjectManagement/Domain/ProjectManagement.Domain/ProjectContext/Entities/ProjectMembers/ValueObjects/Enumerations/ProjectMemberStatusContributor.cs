

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Обычный участник проекта
/// </summary>
public sealed class ProjectMemberStatusContributor : ProjectMemberStatus
{
    public ProjectMemberStatusContributor()
        : base(1, "Участник") { }
}
