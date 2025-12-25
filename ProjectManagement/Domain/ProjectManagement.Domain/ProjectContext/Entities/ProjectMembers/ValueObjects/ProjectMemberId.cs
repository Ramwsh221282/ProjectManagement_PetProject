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

    public static Result<ProjectMemberId, Error> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Failure<ProjectMemberId, Error>(Error.InvalidFormat("Идентификатор участника проекта некорректный."));
        return Success<ProjectMemberId, Error>(new ProjectMemberId(value));
    }
}
