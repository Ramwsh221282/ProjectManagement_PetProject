namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

/// <summary>
/// Информация о задаче (название + описание)
/// </summary>
public sealed record ProjectTaskInfo
{
    /// <summary>
    /// Макс. длина названия задачи
    /// </summary>
    public const int MAX_TITLE_LENGTH = 200;

    /// <summary>
    /// Макс длина описания задачи
    /// </summary>
    public const int MAX_DESCRIPTION_LENGTH = 500;

    /// <summary>
    /// Название задачи
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string Description { get; }

    private ProjectTaskInfo(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public static ProjectTaskInfo Create(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заголовок проекта был пустым.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание проекта было пустым.");

        if (title.Length > MAX_TITLE_LENGTH)
            throw new ArgumentException($"Длина заголовока больше {MAX_TITLE_LENGTH} символов.");

        if (description.Length > MAX_DESCRIPTION_LENGTH)
            throw new ArgumentException($"Описание превышает {MAX_DESCRIPTION_LENGTH} символов.");

        return new ProjectTaskInfo(title, description);
    }
}
