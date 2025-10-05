using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects.Enumerations;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Статусная информация о задаче
/// </summary>
public sealed record ProjectTaskStatusInfo
{
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
