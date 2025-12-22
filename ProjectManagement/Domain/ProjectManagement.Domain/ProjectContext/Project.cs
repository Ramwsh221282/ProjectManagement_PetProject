using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Domain.ProjectContext;

/// <summary>
/// Проект.
/// </summary>
public sealed class Project
{
    private Project() { } // ef core
    
    /// <summary>
    /// Участники проекта.
    /// </summary>
    private List<ProjectMember> _members { get; set; } = [];

    /// <summary>
    /// Задачи проекта.
    /// </summary>
    private List<ProjectTask> _tasks { get; set; } = [];

    /// <summary>
    /// Идентификатор проекта.
    /// </summary>
    public ProjectId Id { get; private set; }
    
    /// <summary>
    /// Владелец проекта.
    /// </summary>
    public ProjectOwnership Ownership { get; private set; }

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
    
    /// <summary>
    /// Участники проекта
    /// </summary>
    public IReadOnlyCollection<ProjectMember> Members => _members;
    
    /// <summary>
    /// Обновить название и описание проекта
    /// </summary>
    /// <param name="name">Новое название проекта</param>
    /// <param name="description">Новое описание проекта</param>
    public void Update(string? name = null, string? description = null)
    {
        if (name is not null)
        {
            ProjectName nextName = ProjectName.Create(name);
            Name = nextName;
        }

        if (description is not null)
        {
            ProjectDescription nextDescription = ProjectDescription.Create(description);
            Description = nextDescription;
        }
    }
    
    /// <summary>
    /// Добавление задачи в проект
    /// </summary>
    /// <param name="task">Задача</param>
    /// <exception cref="InvalidOperationException">Задача уже существует или уже закрыта.</exception>
    public void AddTask(ProjectTask task)
    {
        if (IsFinished()) throw new InvalidOperationException("Проект уже закрыт.");
        if (task.AlreadyExistsIn(_tasks)) throw new InvalidOperationException("Задача уже существует.");
        if (task.IsClosed()) throw new InvalidOperationException("Нельзя добавить закрытую задачу в проект.");
        
        task.SignInProject(this);
        _tasks.Add(task);
    }
    
    /// <summary>
    /// Проверка, завершен ли проект
    /// </summary>
    /// <returns>True, если проект завершен</returns>
    public bool IsFinished()
    {
        return LifeTime.IsFinished;
    }
    
    /// <summary>
    /// Формирование назначения участника на задачу
    /// </summary>
    /// <param name="task">Задача</param>
    /// <param name="member">Участник</param>
    /// <returns>Назначение участника на задачу</returns>
    /// <exception cref="InvalidOperationException">Проект или задача уже закрыты.</exception>
    public ProjectTaskAssignment FormAssignment(ProjectTask task, ProjectMember member)
    {
        if (IsFinished()) throw new InvalidOperationException("Проект уже закрыт.");
        if (task.IsClosed()) throw new InvalidOperationException("Задача уже закрыта.");
        ProjectTaskAssignment assignment = ProjectTaskAssignment.FormAssignmentByCurrentDate(task, member);
        task.AddAssignment(assignment);
        member.AssignTo(assignment);
        return assignment;
    }
    
    /// <summary>
    /// Поиск задачи в проекте
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <returns>Найденная задача</returns>
    /// <exception cref="InvalidOperationException">Задача не найдена.</exception>
    public ProjectTask FindTask(Guid id)
    {
        ProjectTask? task = _tasks.FirstOrDefault(t => t.Id.Value == id);
        return task ?? throw new InvalidOperationException("Задача не найдена.");
    }
    
    /// <summary>
    /// Поиск участника в проекте
    /// </summary>
    /// <param name="id">Идентификатор участника</param>
    /// <returns>Найденный участник</returns>
    /// <exception cref="InvalidOperationException">Участник не найден.</exception>
    public ProjectMember FindMember(Guid id)
    {
        ProjectMember? member = _members.FirstOrDefault(m => m.MemberId.Value == id);
        return member ?? throw new InvalidOperationException("Участник не найден.");
    }
    
    /// <summary>
    /// Закрытие задачи в проекте
    /// </summary>
    /// <param name="task">Задача</param>
    /// <exception cref="InvalidOperationException">Задача не принадлежит проекту или уже закрыта.</exception>
    public void CloseTask(ProjectTask task)
    {
        if (!task.BelongsTo(this)) throw new InvalidOperationException("Задача не принадлежит проекту.");
        
        if (task.IsClosed()) throw new InvalidOperationException("Задача уже закрыта.");
        
        task.Close();
    }
    
    /// <summary>
    /// Добавление нескольких задач в проект
    /// </summary>
    /// <param name="tasks">Список задач</param>
    public void AddTasks(IEnumerable<ProjectTask> tasks)
    {
        foreach (ProjectTask task in tasks)
            AddTask(task);
    }
    
    /// <summary>
    /// Закрытие проекта
    /// </summary>
    /// <exception cref="InvalidOperationException">Проект уже закрыт.</exception>
    public void Close()
    {
        if (IsFinished()) throw new InvalidOperationException("Проект уже закрыт.");
        
        ProjectLifeTime life = LifeTime.Closed(DateTime.UtcNow);
        
        LifeTime = life;
    }
    
    /// <summary>
    /// Добавление участника в проект
    /// </summary>
    /// <param name="member">Участник проекта</param>
    /// <exception cref="InvalidOperationException">Участник уже существует.</exception>
    public void AddMember(ProjectMember member)
    {
        if (member.ExistsIn(_members)) throw new InvalidOperationException("Участник уже существует.");
        member.JoinTo(this);
        _members.Add(member);
    }
    
    /// <summary>
    /// Добавление нескольких участников в проект
    /// </summary>
    /// <param name="members">Список участников</param>
    public void AddMembers(IEnumerable<ProjectMember> members)
    {
        foreach (ProjectMember member in members)
            AddMember(member);
    }
    
    /// <summary>
    /// Создание нового проекта каким-то пользователем.
    /// </summary>
    /// <param name="name">Название проекта</param>
    /// <param name="description">Описание проекта</param>
    /// <param name="user">Пользователь, создающий проект</param>
    /// <param name="approval">Результат проверки уникальности названия проекта</param>
    /// <returns>Созданный проект</returns>
    /// <exception cref="InvalidOperationException">Проект с таким названием уже существует.</exception>
    public static Project CreateNew(
        ProjectName name, 
        ProjectDescription description, 
        User user, 
        ProjectRegistrationApproval approval)
    {
        if (!approval.HasUniqueName)
            throw new InvalidOperationException("Проект с таким названием уже существует.");
        
        ProjectId projectId = ProjectId.Create(Guid.NewGuid());
        ProjectMemberId ownerId = ProjectMemberId.Create(user.UserId.Value);
        ProjectMemberLogin ownerLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        
        ProjectMember owner = ProjectMember.CreateOwner(ownerId, ownerLogin);
        ProjectOwnership ownership = new ProjectOwnership(projectId, user);
        
        Project project = new Project()
        {
            Id = projectId,
            Name = name,
            Description = description,
            Ownership = ownership,
            LifeTime = ProjectLifeTime.Create(DateTime.UtcNow, null),
            _members = [],
            _tasks = [],
        };
        
        project.AddMember(owner);
        return project;
    }
}
