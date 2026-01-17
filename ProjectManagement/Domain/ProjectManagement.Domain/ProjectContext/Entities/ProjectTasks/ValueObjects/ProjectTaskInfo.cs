using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

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

    private ProjectTaskInfo()
    {
        Title = null!;
        Description = null!;
    } // ef core

    public static Result<ProjectTaskInfo> Create(string title, string description) =>
        (title, description) switch
        {
            { title: var t, description: _ } when string.IsNullOrWhiteSpace(t) =>
                Error.InvalidFormat("Заголовок задачи был пустым."),
            { title: _, description: var d } when string.IsNullOrWhiteSpace(d) =>
                Error.InvalidFormat("Описание задачи было пустым."),
            { title: var t, description: _ } when t.Length > MAX_TITLE_LENGTH =>
                Error.InvalidFormat($"Длина заголовка задачи больше {MAX_TITLE_LENGTH} символов."),
            { title: _, description: var d } when d.Length > MAX_DESCRIPTION_LENGTH =>
                Error.InvalidFormat(
                    $"Длина описания задачи больше {MAX_DESCRIPTION_LENGTH} символов."
                ),
            { title: var t, description: var d } => new ProjectTaskInfo(t, d),
        };
}
