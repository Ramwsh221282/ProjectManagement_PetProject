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

    private ProjectTaskInfo() { } // ef core
 
    public static Result<ProjectTaskInfo, Error> Create(string title, string description)
    {
        Func<Result<ProjectTaskInfo, Error>> operation = (title, description) switch
        {
            { title: var t, description: var d } when string.IsNullOrWhiteSpace(t) => 
                () => Failure<ProjectTaskInfo, Error>(Error.InvalidFormat("Заголовок задачи был пустым.")),
            { title: var t, description: var d } when string.IsNullOrWhiteSpace(d) => 
                () => Failure<ProjectTaskInfo, Error>(Error.InvalidFormat("Описание задачи было пустым.")),
            { title: var t, description: var d } when t.Length > MAX_TITLE_LENGTH => 
                () => Failure<ProjectTaskInfo, Error>(Error.InvalidFormat($"Длина заголовка задачи больше {MAX_TITLE_LENGTH} символов.")),
            { title: var t, description: var d } when d.Length > MAX_DESCRIPTION_LENGTH => 
                () => Failure<ProjectTaskInfo, Error>(Error.InvalidFormat($"Длина описания задачи больше {MAX_DESCRIPTION_LENGTH} символов.")),
            { title: var t, description: var d } => 
                () => Success<ProjectTaskInfo, Error>(new ProjectTaskInfo(t, d)),
        };
        return operation();
    }
}
