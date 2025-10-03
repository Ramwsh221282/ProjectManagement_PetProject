namespace ProjectManagement.Domain.ProjectContexts.ValueObjects;

public sealed record ProjectLifeTime
{
    public DateOnly CreatedAt { get; }
    public DateOnly? FinishedAt { get; }

    public ProjectLifeTime()
    {
        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        FinishedAt = null;
    }

    private ProjectLifeTime(DateOnly createdAt, DateOnly? finishedAt)
    {
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
    }

    public static ProjectLifeTime Create(DateOnly createdAt, DateOnly? finishedAt)
    {
        if (createdAt == DateOnly.MaxValue)
            throw new ArgumentException("Некорректная дата начала проекта.");

        if (createdAt == DateOnly.MinValue)
            throw new ArgumentException("Некорректная дата начала проекта");

        if (finishedAt == null) 
            return new ProjectLifeTime(createdAt, finishedAt);
        
        if (createdAt > finishedAt)
            throw new ArgumentException("Дата завершения проекта не может быть больше даты начала.");

        return new ProjectLifeTime(createdAt, finishedAt);
    }
}