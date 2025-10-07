namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Лимит участников задачи
/// </summary>
public readonly record struct ProjectTaskMembersLimit
{
    /// <summary>
    /// Значение лимита участников задачи
    /// </summary>
    public short Value { get; }

    public ProjectTaskMembersLimit() => Value = 0;

    private ProjectTaskMembersLimit(short value) => Value = value;

    public static ProjectTaskMembersLimit Create(short value) =>
        value < 0
            ? throw new ArgumentException("Количество участников задачи не может быть больше 0")
            : new ProjectTaskMembersLimit(value);
}
