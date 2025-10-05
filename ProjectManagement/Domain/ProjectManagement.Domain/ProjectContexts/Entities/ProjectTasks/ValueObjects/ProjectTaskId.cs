namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

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

    public static ProjectTaskId Create(Guid value) =>
        value == Guid.Empty
            ? throw new ArgumentException("Некоррекнтый идентификатор задачи проекта.")
            : new ProjectTaskId(value);
}
