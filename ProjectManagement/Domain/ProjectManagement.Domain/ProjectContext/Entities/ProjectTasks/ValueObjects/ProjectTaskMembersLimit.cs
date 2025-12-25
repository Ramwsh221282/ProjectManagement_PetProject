using ProjectManagement.Domain.Utilities;

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

    public static Result<ProjectTaskMembersLimit, Error> Create(short value) =>
        value < 0
            ? Failure<ProjectTaskMembersLimit, Error>(Error.InvalidFormat("Количество участников задачи не может быть больше 0"))
            : Success<ProjectTaskMembersLimit, Error>(new ProjectTaskMembersLimit(value));
}
