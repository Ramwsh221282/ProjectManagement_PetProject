using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;

/// <summary>
/// Задача проекта
/// </summary>
public sealed class ProjectTask
{
    private ProjectTask() { } // ef core

    /// <summary>
    /// Проект
    /// </summary>
    public Project? Project { get; private set; }

    /// <summary>   
    /// Идентификатор проекта
    /// </summary>
    public ProjectId? ProjectId { get; private set; }

    /// <summary>
    /// Участники задачи
    /// </summary>
    private readonly List<ProjectTaskAssignment> _assignments = [];

    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public ProjectTaskId Id { get; private set; }

    /// <summary>
    /// Лимит участников задачи
    /// </summary>
    public ProjectTaskMembersLimit Limit { get; private set; }

    /// <summary>
    /// Статус задачи
    /// </summary>
    public ProjectTaskStatusInfo StatusInfo { get; private set; }

    /// <summary>
    /// Информация о задаче
    /// </summary>
    public ProjectTaskInfo Information { get; private set; }

    /// <summary>
    /// Участники задачи (коллекция только для чтения)
    /// </summary>
    public IReadOnlyList<ProjectTaskAssignment> Assignments => _assignments;

    public ProjectTask(
        ProjectTaskId id,
        ProjectTaskMembersLimit limit,
        ProjectTaskStatusInfo statusInfo,
        ProjectTaskInfo information,
        IEnumerable<ProjectTaskAssignment>? assignments
    )
    {
        _assignments = assignments == null ? [] : [..assignments];
        Id = id;
        Limit = limit;
        Information = information;
        StatusInfo = statusInfo;
    }

    public void SignInProject(Project project)
    {
        Project = project;
        ProjectId = project.Id;
    }
    
    public void Close()
    {
        ProjectTaskStatusInfo status = new ProjectTaskStatusInfo(
            new ProjectTaskStatusClosed(),
            StatusInfo.Schedule);
        StatusInfo = status;
    }
    
    public void AddAssignment(ProjectTaskAssignment assignment)
    {
        _assignments.Add(assignment);
    }

    public bool BelongsTo(Project project)
    {
        if (ProjectId == null) return false;
        return ProjectId.Value == project.Id;
    }
    
    public bool EqualsByTitle(ProjectTask other)
    {
        return Information.Title == other.Information.Title;
    }

    public bool EqualsById(ProjectTask other)
    {
        return Id == other.Id;
    }
    
    public bool IsClosed()
    {
        return StatusInfo.Status.Value == new ProjectTaskStatusClosed().Value;
    }

    public bool AlreadyExistsIn(IEnumerable<ProjectTask> source)
    {
        return source.Any(t => t.EqualsByTitle(this) || t.EqualsById(this));
    }

    public static ProjectTask CreateNew(
        ProjectTaskMembersLimit membersLimit,
        ProjectTaskInfo information,
        ProjectTaskSchedule schedule
        )
    {
        var id = new ProjectTaskId();
        var status = new ProjectTaskStatusOpened();
        ProjectTaskStatusInfo statusInfo = new(status, schedule);
        return new ProjectTask(id, membersLimit, statusInfo, information, []);
    }
}
