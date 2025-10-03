namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

public sealed class ProjectTaskStatusClosed : ProjectTaskStatus
{
    public ProjectTaskStatusClosed() : base(0, "Закрыта")
    {
    }

    public override bool CanBeRedacted() => false;
}