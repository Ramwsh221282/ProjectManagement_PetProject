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
    public ProjectLifeTime LifeTime { get; private set; }

    /// <summary>
    /// Описание проекта
    /// </summary>
    public ProjectDescription Description { get; private set; }

    /// <summary>
    /// Название проекта
    /// </summary>
    public ProjectName Name { get; private set; }

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

    public void Update(string? name = null, string? description = null)
    {
        if (name is not null)
        {
            var nextName = ProjectName.Create(name);
            Name = nextName;
        }

        if (description is not null)
        {
            var nextDescription = ProjectDescription.Create(description);
            Description = nextDescription;
        }
    }

    public void AddTask(ProjectTask task)
    {
        _tasks.Add(task);
    }
    
    public void Close()
    {
        if (LifeTime.IsFinished)
            throw new InvalidOperationException("Проект уже закрыт.");
        
        ProjectLifeTime life = LifeTime.Closed(DateTime.UtcNow);
        LifeTime = life;
    }

    public void AddMember(ProjectMember member)
    {
        _members.Add(member);
    }
}
