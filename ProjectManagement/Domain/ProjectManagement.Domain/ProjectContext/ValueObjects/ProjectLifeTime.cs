namespace ProjectManagement.Domain.ProjectContext.ValueObjects;

/// <summary>
/// Жизненный цикл проекта
/// </summary>
public sealed record ProjectLifeTime
{
    /// <summary>
    /// Дата создания проекта
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Дата окончания проекта
    /// </summary>
    public DateTime? FinishedAt { get; }

    public ProjectLifeTime()
    {
        CreatedAt = DateTime.UtcNow;
        FinishedAt = null;
    }

    public bool IsFinished => FinishedAt != null && FinishedAt.Value < DateTime.UtcNow;
    
    private ProjectLifeTime(DateTime createdAt, DateTime? finishedAt)
    {
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
    }

    public ProjectLifeTime Closed(DateTime closedAt)
    {
        return new ProjectLifeTime(CreatedAt, closedAt);
    }
    
    public static ProjectLifeTime Create(DateOnly createdAt, DateOnly? finishedAt)
    {
        if (createdAt == DateOnly.MaxValue)
            throw new ArgumentException("Некорректная дата начала проекта.");

        if (createdAt == DateOnly.MinValue)
            throw new ArgumentException("Некорректная дата начала проекта");
        
        DateTime createdAtDt = createdAt.ToDateTime(new TimeOnly());
        if (finishedAt == null)
        {
            return new ProjectLifeTime(createdAtDt, null);
        }
        
        DateTime finished = finishedAt.Value.ToDateTime(new TimeOnly());
        
        if (createdAt > finishedAt)
            throw new ArgumentException(
                "Дата завершения проекта не может быть больше даты начала."
            );
        
        return new ProjectLifeTime(createdAtDt, finished);
    }
}
