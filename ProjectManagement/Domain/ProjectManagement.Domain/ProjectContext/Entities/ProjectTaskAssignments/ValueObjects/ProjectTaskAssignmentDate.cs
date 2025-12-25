using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments.ValueObjects;

/// <summary>
/// Дата назначения участника в задачу
/// </summary>
public readonly record struct ProjectTaskAssignmentDate
{
    public DateTime AssignedAt { get; }

    public ProjectTaskAssignmentDate()
    {
        AssignedAt = DateTime.UtcNow;
    }

    private ProjectTaskAssignmentDate(DateTime date)
    {
        AssignedAt = date;
    }

    public static Result<ProjectTaskAssignmentDate, Error> Create(DateTime date, ProjectTask task)
    {
        Func<Result<ProjectTaskAssignmentDate, Error>> operation = (date, task) switch
        {
            { date: var d, task: var t } when d == DateTime.MinValue => () =>
                Failure<ProjectTaskAssignmentDate, Error>(Error.InvalidFormat("Дата назначения участника в задачу некорректна.")),
            { date: var d, task: var t } when d == DateTime.MaxValue => () =>
                Failure<ProjectTaskAssignmentDate, Error>(Error.InvalidFormat("Дата назначения участника в задачу некорректна.")),
            { date: var d, task: var t } when t.StatusInfo.Schedule.Created < d => () =>
                Failure<ProjectTaskAssignmentDate, Error>(Error.InvalidFormat("Дата назначения участника в задачу некорректна. Дата начала задачи меньше даты назначения.")),
            { date: var d, task: var t } => () =>
                Success<ProjectTaskAssignmentDate, Error>(new ProjectTaskAssignmentDate(d)),
        };
        
        return operation();
    }

    public static ProjectTaskAssignmentDate Current()
    {
        return new ProjectTaskAssignmentDate(DateTime.UtcNow);
    }
    
    public static Result<ProjectTaskAssignmentDate, Error> Create(DateTime date)
    {
        Func<Result<ProjectTaskAssignmentDate, Error>> operation = date switch
        {
            { } d when d == DateTime.MinValue => () => 
                Failure<ProjectTaskAssignmentDate, Error>(Error.InvalidFormat("Дата назначения участника в задачу некорректна.")),
            { } d when d == DateTime.MaxValue => () => 
                Failure<ProjectTaskAssignmentDate, Error>(Error.InvalidFormat("Дата назначения участника в задачу некорректна.")),
            _ => () => Success<ProjectTaskAssignmentDate, Error>(new ProjectTaskAssignmentDate(date)), 
        };
        return operation();
    }
}
