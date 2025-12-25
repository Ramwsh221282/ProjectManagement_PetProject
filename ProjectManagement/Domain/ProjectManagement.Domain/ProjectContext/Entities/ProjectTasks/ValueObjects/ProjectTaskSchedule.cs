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

    public static Result<ProjectTaskSchedule, Error> Create(DateTime created, DateTime? closed)
    {
        Func<Result<ProjectTaskSchedule, Error>> operation = (created, closed) switch
        {
            { created: var c, closed: null } when c == DateTime.MaxValue => ()
                => Failure<ProjectTaskSchedule, Error>(Error.InvalidFormat("Дата начала задачи некорректна.")),
            { created: var c, closed: null } when c == DateTime.MinValue => ()
                => Failure<ProjectTaskSchedule, Error>(Error.InvalidFormat("Дата конца задачи некорректна.")),
            { created: var c, closed: var cl } when cl == DateTime.MaxValue => ()
                => Failure<ProjectTaskSchedule, Error>(Error.InvalidFormat("Дата начала задачи некорректна.")),
            { created: var c, closed: var cl } when cl == DateTime.MinValue => ()
                => Failure<ProjectTaskSchedule, Error>(Error.InvalidFormat("Дата конца задачи некорректна.")),
            { created: var c, closed: var cl } when cl < c => () => 
                Failure<ProjectTaskSchedule, Error>(Error.InvalidFormat("Дата окончания задачи менее даты начала задачи.")),
            { created: var c, closed: var cl } => () => Success<ProjectTaskSchedule, Error>(new ProjectTaskSchedule(c, cl)),
        };
        
        return operation();
    }
}
