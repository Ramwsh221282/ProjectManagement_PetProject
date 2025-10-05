using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMemberAssignments.ValueObjects;

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

    public static ProjectTaskAssignmentDate Create(DateTime date, ProjectTask task)
    {
        if (date == DateTime.MinValue)
            throw new ArgumentException("Дата назначения участника в задачу некорректна");

        if (date == DateTime.MaxValue)
            throw new ArgumentException("Дата назначения участника в задачу некорректна");

        if (task.StatusInfo.Schedule.Created < date)
            throw new ArgumentException(
                "Дата назначения участника в задачу некорректна. Дата начала задачи меньше даты назначения."
            );

        if (task.Project.LifeTime.CreatedAt.ToDateTime(TimeOnly.MinValue) < date)
            throw new ArgumentException(
                "Дата назначения участника в задачу некорректна. Дата начала проекта меньше даты назначения."
            );

        return new ProjectTaskAssignmentDate(date);
    }

    public static ProjectTaskAssignmentDate Create(DateTime date)
    {
        if (date == DateTime.MinValue)
            throw new ArgumentException("Дата назначения участника в задачу некорректна");

        if (date == DateTime.MaxValue)
            throw new ArgumentException("Дата назначения участника в задачу некорректна");

        return new ProjectTaskAssignmentDate(date);
    }
}
