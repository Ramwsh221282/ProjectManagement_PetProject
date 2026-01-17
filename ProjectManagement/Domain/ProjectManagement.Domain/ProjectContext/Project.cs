using ProjectManagement.Domain.ProjectContext;
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
    private Project()
    {
        Id = default!;
        Ownership = default!;
        LifeTime = default!;
        Description = default!;
        Name = default!;
        Description = default!;
    } // ef core

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
    public Result Update(string? name = null, string? description = null)
    {
        var pName = name is null ? null : ProjectName.Create(name);
        if (pName is not null)
        {
            if (pName.IsFailure)
                return pName.OnError;
            Name = pName.OnSuccess;
        }

        var pDescription = description is null ? null : ProjectDescription.Create(description);
        if (pDescription is not null)
        {
            if (pDescription.IsFailure)
                return pDescription.OnError;
            Description = pDescription.OnSuccess;
        }

        return Success();
    }

    /// <summary>
    /// Добавление задачи в проект
    /// </summary>
    /// <param name="task">Задача</param>
    public Result<Unit> AddTask(ProjectTask task)
    {
        (bool finished, bool exists, bool closed) = (
            IsFinished(),
            task.AlreadyExistsIn(_tasks),
            task.IsClosed()
        );
        Func<Result<Unit>> operation = (finished, exists, closed) switch
        {
            (true, _, _) => () => Error.Conflict("Проект уже закрыт."),
            (_, true, _) => () => Error.Conflict("Задача уже существует."),
            (_, _, true) => () => Error.Conflict("Нельзя добавить закрытую задачу в проект."),
            _ => () =>
            {
                task.SignInProject(this);
                _tasks.Add(task);
                return Unit.Value;
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
    public Result<ProjectTaskAssignment> FormAssignment(ProjectTask task, ProjectMember member)
    {
        if (IsFinished())
            return Error.Conflict("Проект уже закрыт.");
        if (task.IsClosed())
            return Error.Conflict("Задача уже закрыта.");

        var assignment = ProjectTaskAssignment.FormAssignmentByCurrentDate(task, member);
        task.AddAssignment(assignment);
        member.AssignTo(assignment);
        return assignment;
    }

    /// <summary>
    /// Поиск задачи в проекте
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <returns>Найденная задача</returns>
    public Result<ProjectTask> FindTask(Guid id)
    {
        ProjectTask? task = _tasks.FirstOrDefault(t => t.Id.Value == id);
        return task is null ? Error.NotFound("Задача не найдена.") : task;
    }

    /// <summary>
    /// Поиск участника в проекте
    /// </summary>
    /// <param name="id">Идентификатор участника</param>
    /// <returns>Найденный участник</returns>
    public Result<ProjectMember> FindMember(Guid id)
    {
        ProjectMember? member = _members.FirstOrDefault(m => m.MemberId.Value == id);
        return member is null ? Error.NotFound("Участник не найден.") : member;
    }

    /// <summary>
    /// Закрытие задачи в проекте
    /// </summary>
    /// <param name="task">Задача</param>
    public Result<Unit> CloseTask(ProjectTask task) =>
        !task.BelongsTo(this) ? Error.Conflict("Задача не принадлежит проекту.") : task.Close();

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
    public Result<Unit> Close()
    {
        if (IsFinished())
            return Error.Conflict("Проект уже закрыт.");

        ProjectLifeTime life = LifeTime.Closed(DateTime.UtcNow);
        LifeTime = life;
        return Unit.Value;
    }

    /// <summary>
    /// Добавление участника в проект
    /// </summary>
    /// <param name="member">Участник проекта</param>
    public Result<Unit> AddMember(ProjectMember member)
    {
        if (member.ExistsIn(_members))
            return Error.Conflict("Участник уже существует.");

        member.JoinTo(this);
        _members.Add(member);
        return Unit.Value;
    }

    /// <summary>
    /// Добавление нескольких участников в проект
    /// </summary>
    /// <param name="members">Список участников</param>
    public Result<Unit> AddMembers(IEnumerable<ProjectMember> members)
    {
        foreach (ProjectMember member in members)
        {
            if (member.ExistsIn(_members))
                return Error.Conflict("Участник уже существует.");

            member.JoinTo(this);
            _members.Add(member);
        }
        return Unit.Value;
    }

    /// <summary>
    /// Создание нового проекта каким-то пользователем.
    /// </summary>
    /// <param name="name">Название проекта</param>
    /// <param name="description">Описание проекта</param>
    /// <param name="user">Пользователь, создающий проект</param>
    /// <param name="approval">Результат проверки уникальности названия проекта</param>
    /// <returns>Созданный проект</returns>
    public static Result<Project> CreateNew(
        ProjectName name,
        ProjectDescription description,
        User user,
        ProjectRegistrationApproval approval
    )
    {
        if (!approval.HasUniqueName)
            return Error.Conflict("Проект с таким названием уже существует.");

        ProjectId projectId = new();
        ProjectMemberId ownerId = ProjectMemberId.Create(user.UserId.Value).OnSuccess;
        ProjectMemberLogin ownerLogin = ProjectMemberLogin.Create(user.AccountData.Login).OnSuccess;
        ProjectMember owner = ProjectMember.CreateOwner(ownerId, ownerLogin);
        ProjectOwnership ownership = new ProjectOwnership(projectId, user);
        ProjectLifeTime lifeTime = ProjectLifeTime.Create(DateTime.UtcNow, null).OnSuccess;

        Project project = new()
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
        return project;
    }
}
