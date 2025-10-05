namespace ProjectManagement.Domain.ProjectContexts.ValueObjects;

/// <summary>
/// Идентификатор проекта
/// </summary>
public readonly record struct ProjectId
{
    public Guid Value { get; }

    public ProjectId() => Value = Guid.NewGuid();

    public ProjectId(Guid value) => Value = value;

    public static ProjectId Create(Guid value) =>
        value == Guid.Empty
            ? throw new ArgumentException("Идентификатор проекта некорректный.")
            : new ProjectId(value);
}
