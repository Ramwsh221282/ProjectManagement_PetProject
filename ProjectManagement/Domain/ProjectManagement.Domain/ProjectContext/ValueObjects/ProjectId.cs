using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.ValueObjects;

/// <summary>
/// Идентификатор проекта
/// </summary>
public readonly record struct ProjectId
{
    public Guid Value { get; }

    public ProjectId() => Value = Guid.NewGuid();

    public ProjectId(Guid value) => Value = value;

    public static Result<ProjectId> Create(Guid value) =>
        value switch
        {
            { } when value == Guid.Empty => Error.InvalidFormat(
                "Идентификатор проекта некорректный."
            ),
            { } => new ProjectId(value),
        };
}
