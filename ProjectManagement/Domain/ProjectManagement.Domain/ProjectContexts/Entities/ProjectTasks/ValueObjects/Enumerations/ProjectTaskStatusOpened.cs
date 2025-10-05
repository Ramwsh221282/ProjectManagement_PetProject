namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

/// <summary>
/// Открытый статус задачи
/// </summary>
public sealed class ProjectTaskStatusOpened : ProjectTaskStatus
{
    public ProjectTaskStatusOpened()
        : base(0, "Открыта") { }

    public override bool CanBeRedacted() => true;
}
