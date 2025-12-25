using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Идентификатор задачи
/// </summary>
public readonly record struct ProjectTaskId
{
    public Guid Value { get; }

    public ProjectTaskId()
    {
        Value = Guid.NewGuid();
    }

    private ProjectTaskId(Guid value)
    {
        Value = value;
    }

    public static Result<ProjectTaskId, Error> Create(Guid value) =>
        value == Guid.Empty
            ? Failure<ProjectTaskId, Error>(Error.InvalidFormat("Некорректный идентификатор задачи проекта."))
            : Success<ProjectTaskId, Error>(new ProjectTaskId(value));
}
