using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Статусная информация о задаче
/// </summary>
public sealed record ProjectTaskStatusInfo
{
    private ProjectTaskStatusInfo() { } // ef core

    /// <summary>
    /// Статус задачи
    /// </summary>
    public ProjectTaskStatus Status { get; }

    /// <summary>
    /// Сроки задачи
    /// </summary>
    public ProjectTaskSchedule Schedule { get; }

    public ProjectTaskStatusInfo(ProjectTaskStatus status, ProjectTaskSchedule schedule)
    {
        Status = status;
        Schedule = schedule;
    }
}
