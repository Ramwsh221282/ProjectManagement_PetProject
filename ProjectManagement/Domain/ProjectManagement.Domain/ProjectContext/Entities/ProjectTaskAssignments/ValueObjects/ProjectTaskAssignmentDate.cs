using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments.ValueObjects;

/// <summary>
/// Дата назначения участника в задачу
/// </summary>
public readonly record struct ProjectTaskAssignmentDate
{
    public DateTime AssignedAt { get; }

    public ProjectTaskAssignmentDate() => AssignedAt = DateTime.UtcNow;

    private ProjectTaskAssignmentDate(DateTime date) => AssignedAt = date;

    public static Result<ProjectTaskAssignmentDate> Create(DateTime date, ProjectTask task) =>
        (date, task) switch
        {
            { date: var d, task: _ } when d == DateTime.MinValue => Error.InvalidFormat(
                "Дата назначения участника в задачу некорректна."
            ),
            { date: var d, task: _ } when d == DateTime.MaxValue => Error.InvalidFormat(
                "Дата назначения участника в задачу некорректна."
            ),
            { date: var d, task: var t } when t.StatusInfo.Schedule.Created < d =>
                Error.InvalidFormat(
                    "Дата назначения участника в задачу некорректна. Дата начала задачи меньше даты назначения."
                ),
            { date: var d, task: _ } => new ProjectTaskAssignmentDate(d),
        };

    public static ProjectTaskAssignmentDate Current() => new(DateTime.UtcNow);

    public static Result<ProjectTaskAssignmentDate> Create(DateTime date) =>
        date switch
        {
            { } d when d == DateTime.MinValue => Error.InvalidFormat(
                "Дата назначения участника в задачу некорректна."
            ),
            { } d when d == DateTime.MaxValue => Error.InvalidFormat(
                "Дата назначения участника в задачу некорректна."
            ),
            _ => new ProjectTaskAssignmentDate(date),
        };
}
