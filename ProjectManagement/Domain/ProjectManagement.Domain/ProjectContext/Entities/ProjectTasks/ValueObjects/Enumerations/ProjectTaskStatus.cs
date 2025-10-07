using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;

/// <summary>
/// Семейство статусов задачи (умное перечисление)
/// </summary>
public abstract class ProjectTaskStatus : Enumeration<ProjectTaskStatus>
{
    protected ProjectTaskStatus(int value, string name)
        : base(value, name) { }

    public abstract bool CanBeRedacted();
}
