namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

public sealed record ProjectTaskInfo
{
    public const int MAX_TITLE_LENGTH = 200;
    public const int MAX_DESCRIPTION_LENGTH = 500;
    public string Title { get; }
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