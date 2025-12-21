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

    public static ProjectTaskAssignmentId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Идентификатор назначения участника в задачу некорректный.");
        return new ProjectTaskAssignmentId(value);
    }

    public static ProjectTaskAssignmentId New()
    {
        return new ProjectTaskAssignmentId(Guid.NewGuid());
    }
}