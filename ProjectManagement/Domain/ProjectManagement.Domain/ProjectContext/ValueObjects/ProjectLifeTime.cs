using System.Diagnostics;
using ProjectManagement.Domain.Utilities;

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

    private ProjectLifeTime()
    {
    } // ef core

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

    public static Result<ProjectLifeTime, Error> Create(DateTime createdAt, DateTime? finishedAt)
    {
        ErrorResult<ProjectLifeTime> result = (createdAt, finishedAt) switch
        {
            { createdAt: var created, finishedAt: null } => (created) switch
            {
                { } when created == DateTime.MaxValue => Error.InvalidFormat("Некорректная дата начала проекта."),
                { } when created == DateTime.MinValue => Error.InvalidFormat("Некорректная дата начала проекта."),
                { } => new ProjectLifeTime(created, null),
            },

            { createdAt: var created, finishedAt: var closed } => (created, closed) switch
            {
                { } when created == DateTime.MaxValue => Error.InvalidFormat("Некорректная дата начала проекта."),
                { } when created == DateTime.MinValue => Error.InvalidFormat("Некорректная дата начала проекта."),
                { } when closed == DateTime.MaxValue => Error.InvalidFormat("Некорректная дата окончания проекта."),
                { } when closed == DateTime.MinValue => Error.InvalidFormat("Некорректная дата окончания проекта."),
                { } when created > closed => Error.InvalidFormat(
                    "Дата окончания проекта не может быть раньше даты начала."),
                { } => new ProjectLifeTime(created, closed),
            },
        };
        return result;
    }
}
