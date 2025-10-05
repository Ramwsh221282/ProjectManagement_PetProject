using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects.Enumerations;

/// <summary>
/// Семейство статусов участника проекта
/// </summary>
public abstract class ProjectMemberStatus : Enumeration<ProjectMemberStatus>
{
    protected ProjectMemberStatus(int value, string name)
        : base(value, name) { }
}
