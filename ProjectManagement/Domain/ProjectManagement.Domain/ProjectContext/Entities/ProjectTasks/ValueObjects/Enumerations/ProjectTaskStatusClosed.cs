namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;

/// <summary>
/// Закрытый статус задачи
/// </summary>
public sealed class ProjectTaskStatusClosed : ProjectTaskStatus
{
    public ProjectTaskStatusClosed()
        : base(0, "Закрыта") { }

    public override bool CanBeRedacted() => false;
}
