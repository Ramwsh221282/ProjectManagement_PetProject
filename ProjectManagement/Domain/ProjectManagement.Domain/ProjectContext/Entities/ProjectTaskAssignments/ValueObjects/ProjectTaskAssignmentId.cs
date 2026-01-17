using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments.ValueObjects;

public readonly record struct ProjectTaskAssignmentId
{
    public Guid Value { get; }

    public ProjectTaskAssignmentId() => Value = Guid.NewGuid();

    private ProjectTaskAssignmentId(Guid value) => Value = value;

    public static Result<ProjectTaskAssignmentId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.InvalidFormat("Идентификатор назначения участника в задачу некорректный.")
            : new ProjectTaskAssignmentId(value);

    public static ProjectTaskAssignmentId New() => new(Guid.NewGuid());
}
