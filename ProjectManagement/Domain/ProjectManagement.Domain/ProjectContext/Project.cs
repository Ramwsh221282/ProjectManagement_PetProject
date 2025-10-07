using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Domain.ProjectContext;

/// <summary>
/// Проект
/// </summary>
public sealed class Project
{
    private readonly List<ProjectMember> _members;

    /// <summary>
    /// Задачи проекта
    /// </summary>
    private readonly List<ProjectTask> _tasks;

    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public ProjectId Id { get; }

    /// <summary>
    /// Жизненный цикл проекта
    /// </summary>
    public ProjectLifeTime LifeTime { get; }

    /// <summary>
    /// Описание проекта
    /// </summary>
    public ProjectDescription Description { get; }

    /// <summary>
    /// Название проекта
    /// </summary>
    public ProjectName Name { get; }

    /// <summary>
    /// Задачи проекта
    /// </summary>
    public IReadOnlyList<ProjectTask> Tasks => _tasks;

    public IReadOnlyCollection<ProjectMember> Members => _members;

    public Project(ProjectDescription description, ProjectName name, ProjectId? id = null)
    {
        Description = description;
        Name = name;
        Id = id ?? new ProjectId();
        LifeTime = new ProjectLifeTime();
        _members = [];
        _tasks = [];
    }

    public Project(
        ProjectDescription description,
        ProjectName name,
        IEnumerable<ProjectTask> tasks,
        IEnumerable<ProjectMember> members,
        ProjectLifeTime lifeTime,
        ProjectId? id = null
    )
        : this(description, name, id)
    {
        _tasks = [.. tasks];
        _members = [.. members];
        LifeTime = lifeTime;
    }
}
