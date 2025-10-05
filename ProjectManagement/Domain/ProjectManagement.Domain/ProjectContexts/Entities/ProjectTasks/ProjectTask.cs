using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMemberAssignments;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContexts.ValueObjects;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks;

/// <summary>
/// Задача проекта
/// </summary>
public sealed class ProjectTask
{
    /// <summary>
    /// Проект
    /// </summary>
    public Project Project { get; }

    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public ProjectId ProjectId { get; }

    /// <summary>
    /// Участники задачи
    /// </summary>
    private readonly List<ProjectTaskAssignment> _assignements = [];

    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public ProjectTaskId Id { get; }

    /// <summary>
    /// Лимит участников задачи
    /// </summary>
    public ProjectTaskMembersLimit Limit { get; }

    /// <summary>
    /// Статус задачи
    /// </summary>
    public ProjectTaskStatusInfo StatusInfo { get; }

    /// <summary>
    /// Информация о задаче
    /// </summary>
    public ProjectTaskInfo Information { get; }

    /// <summary>
    /// Участники задачи (коллекция только для чтения)
    /// </summary>
    public IReadOnlyList<ProjectTaskAssignment> Assignments => _assignements;

    public ProjectTask(
        ProjectTaskId id,
        ProjectTaskMembersLimit limit,
        ProjectTaskStatusInfo statusInfo,
        ProjectTaskInfo information,
        Project project,
        IEnumerable<ProjectTaskAssignment> assignments
    )
    {
        Project = project;
        ProjectId = project.Id;
        _assignements = [.. assignments];
        Id = id;
        Limit = limit;
        Information = information;
        StatusInfo = statusInfo;
    }
}
