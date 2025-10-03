namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

public sealed record ProjectTaskStatusInfo
{
    public ProjectTaskStatuses Status { get; }
    public ProjectTaskSchedule Schedule { get; }

    public ProjectTaskStatusInfo(ProjectTaskStatuses status, ProjectTaskSchedule schedule)
    {
        Status = status;
        Schedule = schedule;
    }
}