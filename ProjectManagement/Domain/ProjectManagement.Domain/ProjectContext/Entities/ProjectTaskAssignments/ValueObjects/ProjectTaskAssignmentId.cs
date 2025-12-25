using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments.ValueObjects;

public readonly record struct ProjectTaskAssignmentId
{
    public Guid Value { get; }

    public ProjectTaskAssignmentId()
    {
        Value = Guid.NewGuid();
    }

    private ProjectTaskAssignmentId(Guid value)
    {
        Value = value;
    }

    public static Result<ProjectTaskAssignmentId, Error> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Failure<ProjectTaskAssignmentId, Error>(Error.InvalidFormat("Идентификатор назначения участника в задачу некорректный."));
        return Success<ProjectTaskAssignmentId, Error>(new ProjectTaskAssignmentId(value));
    }

    public static ProjectTaskAssignmentId New()
    {
        return new ProjectTaskAssignmentId(Guid.NewGuid());
    }
}