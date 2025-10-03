using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

public abstract class ProjectTaskStatus : Enumeration<ProjectTaskStatus>
{
    protected ProjectTaskStatus(int value, string name) : base(value, name)
    {
    }

    public abstract bool CanBeRedacted();
}