namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

public readonly record struct ProjectTaskMembersLimit
{
    public short Value { get; }

    public ProjectTaskMembersLimit() => Value = 0;
    
    private ProjectTaskMembersLimit(short value) => Value = value;

    public static ProjectTaskMembersLimit Create(short value) =>
        value < 0 ? throw new ArgumentException("Количество участников задачи не может быть больше 0") : new ProjectTaskMembersLimit(value);
}