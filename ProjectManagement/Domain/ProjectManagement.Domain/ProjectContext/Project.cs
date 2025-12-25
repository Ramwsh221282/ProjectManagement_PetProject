using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

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
    public Result<Unit, Error> Update(string? name = null, string? description = null)
    {
        Func<Result<Unit, Error>> operation = (name, description) switch
        {
            (null, null) => () => Success<Unit, Error>(Unit.Value),
            (var nextName, null) => () =>
            {
                Result<ProjectName, Error> nameRes = ProjectName.Create(nextName);
                if (nameRes.IsFailure) return Failure<Unit>(nameRes.OnError);
                Name = nameRes.OnSuccess;
                return Success<Unit, Error>(Unit.Value);
            },
            (null, var nextDescription) => () =>
            {
                Result<ProjectDescription, Error> descriptionRes = ProjectDescription.Create(nextDescription);
                if (descriptionRes.IsFailure) return Failure<Unit>(descriptionRes.OnError);
                Description = descriptionRes.OnSuccess;
                return Success<Unit, Error>(Unit.Value);
            },
            (var nextName, var nextDescription) => () =>
            {
                Result<ProjectName, Error> nameRes = ProjectName.Create(nextName);
                if (nameRes.IsFailure) return Failure<Unit>(nameRes.OnError);
                Name = nameRes.OnSuccess;
                
                Result<ProjectDescription, Error> descriptionRes = ProjectDescription.Create(nextDescription);
                if (descriptionRes.IsFailure) return Failure<Unit>(descriptionRes.OnError);
                Description = descriptionRes.OnSuccess;
                
                return Success<Unit, Error>(Unit.Value);                
            },
        };
        
        return operation();
    }
    
    /// <summary>
    /// Добавление задачи в проект
    /// </summary>
    /// <param name="task">Задача</param>
    public Result<Unit, Error> AddTask(ProjectTask task)
    {
        (bool finished, bool exists, bool closed) = (IsFinished(), task.AlreadyExistsIn(_tasks), task.IsClosed());
        Func<Result<Unit, Error>> operation = (finished, exists, closed) switch
        {
            (true, _, _) => () => Failure<Unit>(Error.Conflict("Проект уже закрыт.")),
            (_, true, _) => () => Failure<Unit>(Error.Conflict("Задача уже существует.")),
            (_, _, true) => () => Failure<Unit>(Error.Conflict("Нельзя добавить закрытую задачу в проект.")),
            _ => () =>
            {
                task.SignInProject(this);
                _tasks.Add(task);
                return Success<Unit, Error>(Unit.Value);
            },
        };
        return operation();
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
    public Result<ProjectTaskAssignment, Error> FormAssignment(ProjectTask task, ProjectMember member)
    {
        (bool isFinished, bool taskClosed) = (IsFinished(), task.IsClosed());
        Func<Result<ProjectTaskAssignment, Error>> operation = (isFinished, taskClosed) switch
        {
            (true, _) => () => Failure<ProjectTaskAssignment, Error>(Error.Conflict("Проект уже закрыт.")),
            (_, true) => () => Failure<ProjectTaskAssignment, Error>(Error.Conflict("Задача уже закрыта.")),
            _ => () =>
            {
                ProjectTaskAssignment assignment = ProjectTaskAssignment.FormAssignmentByCurrentDate(task, member);
                task.AddAssignment(assignment);
                member.AssignTo(assignment);
                return Success<ProjectTaskAssignment, Error>(assignment);
            },
        };
        return operation();
    }
    
    /// <summary>
    /// Поиск задачи в проекте
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <returns>Найденная задача</returns>
    public Result<ProjectTask, Nothing> FindTask(Guid id)
    {
        ProjectTask? task = _tasks.FirstOrDefault(t => t.Id.Value == id);
        return task is null 
            ? Failure<ProjectTask, Nothing>(new Nothing()) 
            : Success<ProjectTask, Nothing>(task);
    }
    
    /// <summary>
    /// Поиск участника в проекте
    /// </summary>
    /// <param name="id">Идентификатор участника</param>
    /// <returns>Найденный участник</returns>
    public Result<ProjectMember, Nothing> FindMember(Guid id)
    {
        ProjectMember? member = _members.FirstOrDefault(m => m.MemberId.Value == id);
        return member is null 
            ? Failure<ProjectMember, Nothing>(new Nothing()) 
            : Success<ProjectMember, Nothing>(member);
    }
    
    /// <summary>
    /// Закрытие задачи в проекте
    /// </summary>
    /// <param name="task">Задача</param>
    public Result<Unit, Error> CloseTask(ProjectTask task)
    {
        (bool belongs, bool closed) = (task.BelongsTo(this), task.IsClosed());
        Func<Result<Unit, Error>> operation = (belongs, closed) switch
        {
            (false, _) => () => Failure<Unit>(Error.Conflict("Задача не принадлежит проекту.")),
            (_, true) => () => Failure<Unit>(Error.Conflict("Задача уже закрыта.")),
            _ => () =>
            {
                task.Close();
                return Success<Unit, Error>(Unit.Value);
            },
        };
        return operation();
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
    public Result<Unit, Error> Close()
    {
        Func<Result<Unit, Error>> operation = IsFinished() switch
        {
            true => () => Failure<Unit>(Error.Conflict("Проект уже закрыт.")),
            _ => () =>
            {
                ProjectLifeTime life = LifeTime.Closed(DateTime.UtcNow);
                LifeTime = life;
                return Success<Unit, Error>(Unit.Value);
            },
        };
        return operation();
    }
    
    /// <summary>
    /// Добавление участника в проект
    /// </summary>
    /// <param name="member">Участник проекта</param>
    public Result<Unit, Error> AddMember(ProjectMember member)
    {
        Func<Result<Unit, Error>> operation = () =>
        {
            if (member.ExistsIn(_members)) return Failure<Unit>(Error.Conflict("Участник уже существует."));
            member.JoinTo(this);
            _members.Add(member);
            return Success<Unit, Error>(Unit.Value);
        };
        return operation();
    }
    
    /// <summary>
    /// Добавление нескольких участников в проект
    /// </summary>
    /// <param name="members">Список участников</param>
    public Result<Unit, Error> AddMembers(IEnumerable<ProjectMember> members)
    {
        Func<Result<Unit, Error>> operation = () =>
        {
            foreach (ProjectMember member in members)
            {
                if (member.ExistsIn(_members)) 
                    return Failure<Unit>(Error.Conflict("Участник уже существует."));
                
                member.JoinTo(this);
                _members.Add(member);
            }
            return Success<Unit, Error>(Unit.Value);
        };
        return operation();
    }
    
    /// <summary>
    /// Создание нового проекта каким-то пользователем.
    /// </summary>
    /// <param name="name">Название проекта</param>
    /// <param name="description">Описание проекта</param>
    /// <param name="user">Пользователь, создающий проект</param>
    /// <param name="approval">Результат проверки уникальности названия проекта</param>
    /// <returns>Созданный проект</returns>
    public static Result<Project, Error> CreateNew(
        ProjectName name, 
        ProjectDescription description, 
        User user, 
        ProjectRegistrationApproval approval)
    {
        Func<Result<Project, Error>> operation = approval.HasUniqueName switch
        {
            false => () => Failure<Project, Error>(Error.Conflict("Проект с таким названием уже существует.")),
            _ => () =>
            {
                ProjectId projectId = new();
                ProjectMemberId ownerId = ProjectMemberId.Create(user.UserId.Value).OnSuccess;
                ProjectMemberLogin ownerLogin = ProjectMemberLogin.Create(user.AccountData.Login).OnSuccess;
                ProjectMember owner = ProjectMember.CreateOwner(ownerId, ownerLogin);
                ProjectOwnership ownership = new ProjectOwnership(projectId, user);
                ProjectLifeTime lifeTime = ProjectLifeTime.Create(DateTime.UtcNow, null).OnSuccess;
                
                Project project = new Project()
                {
                    Id = projectId,
                    Name = name,
                    Description = description,
                    Ownership = ownership,
                    LifeTime = lifeTime,
                    _members = [],
                    _tasks = [],
                };
                
                project.AddMember(owner);
                return Success<Project, Error>(project);
            },
        };
        
        return operation();
    }
}
