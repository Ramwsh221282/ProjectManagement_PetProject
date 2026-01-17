using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;

/// <summary>
/// Идентификатор участника проекта
/// </summary>
public readonly record struct ProjectMemberId
{
    public ProjectMemberId() => Value = Guid.NewGuid();

    private ProjectMemberId(Guid value) => Value = value;

    public Guid Value { get; }

    public static Result<ProjectMemberId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.InvalidFormat("Идентификатор участника проекта некорректный.")
            : new ProjectMemberId(value);
}
