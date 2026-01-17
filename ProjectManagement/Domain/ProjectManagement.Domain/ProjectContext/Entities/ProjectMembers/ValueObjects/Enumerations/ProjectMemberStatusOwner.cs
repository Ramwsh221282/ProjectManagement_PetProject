using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Обладатель проекта
/// </summary>
public sealed class ProjectMemberStatusOwner : ProjectMemberStatus
{
    public ProjectMemberStatusOwner()
        : base(0, "Обладатель") { }
}
