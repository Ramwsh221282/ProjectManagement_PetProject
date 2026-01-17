using ProjectManagement.Domain.Utilities;

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

    public static Result<ProjectTaskSchedule> Create(DateTime created, DateTime? closed) =>
        (created, closed) switch
        {
            { created: var c, closed: null } when c == DateTime.MaxValue => Error.InvalidFormat(
                "Дата начала задачи некорректна."
            ),
            { created: var c, closed: null } when c == DateTime.MinValue => Error.InvalidFormat(
                "Дата конца задачи некорректна."
            ),
            { created: _, closed: var cl } when cl == DateTime.MaxValue => Error.InvalidFormat(
                "Дата начала задачи некорректна."
            ),
            { created: _, closed: var cl } when cl == DateTime.MinValue => Error.InvalidFormat(
                "Дата конца задачи некорректна."
            ),
            { created: var c, closed: var cl } when cl < c => Error.InvalidFormat(
                "Дата окончания задачи менее даты начала задачи."
            ),
            { created: var c, closed: var cl } => new ProjectTaskSchedule(c, cl),
        };
}
