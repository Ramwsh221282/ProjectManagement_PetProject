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

    public static Result<ProjectTaskId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.InvalidFormat("Некорректный идентификатор задачи проекта.")
            : new ProjectTaskId(value);
}
