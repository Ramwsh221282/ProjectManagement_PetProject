namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

public sealed class ProjectTaskStatusOpened : ProjectTaskStatus
{
    public ProjectTaskStatusOpened() : base(0, "Открыта")
    {
    }

    public override bool CanBeRedacted() => true;
}