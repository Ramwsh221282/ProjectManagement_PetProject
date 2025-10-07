namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Расписание сроков задачи
/// </summary>
public readonly record struct ProjectTaskSchedule
{
    /// <summary>
    /// Дата начала задачи
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// Дата окончания задачи
    /// </summary>
    public DateTime? Closed { get; }

    public bool IsClosed => Closed.HasValue;

    private ProjectTaskSchedule(DateTime created, DateTime? closed)
    {
        Created = created;
        Closed = closed;
    }

    public static ProjectTaskSchedule Create(DateTime created, DateTime? closed)
    {
        if (created == DateTime.MaxValue)
            throw new ArgumentException("Дата начала задачи некорректна.");

        if (created == DateTime.MinValue)
            throw new ArgumentException("Дата конца задачи некорректна.");

        if (closed == null)
            return new ProjectTaskSchedule(created, closed);

        if (closed < created)
            throw new ArgumentException("Дата окончания задачи менее даты начала задачи.");

        return new ProjectTaskSchedule(created, closed);
    }
}
