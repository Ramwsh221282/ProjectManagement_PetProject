namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects;

public readonly record struct ProjectMemberId
{
    public ProjectMemberId() => Value = Guid.NewGuid();

    private ProjectMemberId(Guid value) => Value = value;
    
    public Guid Value { get; }

    public static ProjectMemberId Create(Guid value) =>
        value == Guid.Empty ? throw new ArgumentException("Идентификатор участника проекта некорректный.") : new ProjectMemberId(value);
}