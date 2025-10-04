using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

public sealed record ProjectTaskStatusInfo
{
    public ProjectTaskStatus Status { get; }
    public ProjectTaskSchedule Schedule { get; }

    public ProjectTaskStatusInfo(ProjectTaskStatus status, ProjectTaskSchedule schedule)
    {
        Status = status;
        Schedule = schedule;
    }
}
