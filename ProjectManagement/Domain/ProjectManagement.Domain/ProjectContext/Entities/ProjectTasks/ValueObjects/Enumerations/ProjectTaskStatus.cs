using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;

/// <summary>
/// Семейство статусов задачи (умное перечисление)
/// </summary>
public class ProjectTaskStatus : Enumeration<ProjectTaskStatus>
{
    protected ProjectTaskStatus(int value, string name)
        : base(value, name) { }

    public virtual bool CanBeRedacted() => false;
}
