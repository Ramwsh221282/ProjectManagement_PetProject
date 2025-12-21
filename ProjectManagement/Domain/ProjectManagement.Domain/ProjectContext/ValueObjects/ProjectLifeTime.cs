namespace ProjectManagement.Domain.ProjectContext.ValueObjects;

/// <summary>
/// Жизненный цикл проекта
/// </summary>
public sealed record ProjectLifeTime
{
    /// <summary>
    /// Дата создания проекта
    /// </summary>
    public DateOnly CreatedAt { get; }

    /// <summary>
    /// Дата окончания проекта
    /// </summary>
    public DateOnly? FinishedAt { get; }

    private ProjectLifeTime() { } // ef core

    public ProjectLifeTime(DateOnly createdAt)
    {
        CreatedAt = createdAt;
        FinishedAt = null;
    }

    public bool IsFinished => FinishedAt != null && FinishedAt.Value < DateOnly.FromDateTime(DateTime.UtcNow);
    
    private ProjectLifeTime(DateOnly createdAt, DateOnly? finishedAt)
    {
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
    }

    public ProjectLifeTime Closed(DateOnly closedAt)
    {
        return new ProjectLifeTime(CreatedAt, closedAt);
    }
    
    public static ProjectLifeTime Create(DateOnly createdAt, DateOnly? finishedAt)
    {
        if (createdAt == DateOnly.MaxValue)
            throw new ArgumentException("Некорректная дата начала проекта.");

        if (createdAt == DateOnly.MinValue)
            throw new ArgumentException("Некорректная дата начала проекта");
        
        if (finishedAt == null)
        {
            return new ProjectLifeTime(createdAt, null);
        }
        
        if (createdAt > finishedAt)
            throw new ArgumentException(
                "Дата завершения проекта не может быть больше даты начала."
            );
        
        return new ProjectLifeTime(createdAt, finishedAt);
    }
}
