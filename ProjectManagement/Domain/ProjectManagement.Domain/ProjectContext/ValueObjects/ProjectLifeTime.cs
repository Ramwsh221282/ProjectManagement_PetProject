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

    private ProjectLifeTime() { } // ef core

    public ProjectLifeTime(DateTime createdAt)
    {
        CreatedAt = createdAt;
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
    
    public static ProjectLifeTime Create(DateTime createdAt, DateTime? finishedAt)
    {
        if (createdAt == DateTime.MaxValue)
            throw new ArgumentException("Некорректная дата начала проекта.");

        if (createdAt == DateTime.MinValue)
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
