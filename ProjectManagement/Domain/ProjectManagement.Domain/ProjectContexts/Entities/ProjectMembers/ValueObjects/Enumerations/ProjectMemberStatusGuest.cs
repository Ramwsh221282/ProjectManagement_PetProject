using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Гость участник проекта
/// </summary>
public sealed class ProjectMemberStatusGuest : Enumeration<ProjectMemberStatus>
{
    public ProjectMemberStatusGuest(int value, string name)
        : base(2, "Гость") { }
}
