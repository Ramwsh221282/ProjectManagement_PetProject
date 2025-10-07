using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Обычный участник проекта
/// </summary>
public sealed class ProjectMemberStatusContributor : Enumeration<ProjectMemberStatus>
{
    public ProjectMemberStatusContributor()
        : base(1, "Участник") { }
}
