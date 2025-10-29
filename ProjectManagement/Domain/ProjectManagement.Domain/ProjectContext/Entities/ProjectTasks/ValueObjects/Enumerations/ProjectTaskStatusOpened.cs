namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;

/// <summary>
/// Открытый статус задачи
/// </summary>
public sealed class ProjectTaskStatusOpened : ProjectTaskStatus
{
    public ProjectTaskStatusOpened()
        : base(1, "Открыта") { }

    public override bool CanBeRedacted() => true;
}
