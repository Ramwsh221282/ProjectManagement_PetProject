using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;
using ProjectManagement.UseCases.Projects.AddProjectMembers;
using ProjectManagement.UseCases.Projects.AddProjectTasks;
using ProjectManagement.UseCases.Projects.AssignMemberToTask;
using ProjectManagement.UseCases.Projects.CloseProjectTask;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;
using ProjectManagement.UseCases.Projects.UpdateProjectInfo;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

/// <summary>
/// Контроллер для работы с проектами
/// </summary>
[ApiController]
[Route("api/projects")]
public class ProjectsController
{
    /// <summary>
    /// Создание проекта
    /// </summary>
    /// <param name="request">Запрос на создание проекта (UserId, Name, Description)</param>
    /// <param name="handler">Обработчик создания проекта</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный проект</returns>
    [HttpPost]
    public async Task<Envelope> Create(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectByUserHandler handler,
        CancellationToken ct
        )
    {
        CreateProjectByUserCommand command = new(
            UserId: userId, 
            ProjectName: request.Name, 
            ProjectDescription: request.Description);
        
        Project project = await handler.Handle(command, ct);
        return new Envelope(project.ToDto());        
    }
    
    /// <summary>
    /// Добавление задач в проект
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="projectId">Идентификатор проекта</param>
    /// <param name="request">Запрос на добавление задач (Tasks)</param>
    /// <param name="handler">Обработчик добавления задач</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Добавленные задачи</returns>
    [HttpPost("{id:guid}/tasks")]
    public async Task<Envelope> AddTasks(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromRoute(Name = "id")] Guid projectId,
        [FromBody] AddProjectTasksRequest request,
        [FromServices] AddProjectTasksHandler handler,
        CancellationToken ct
        )
    {
        AddProjectTasksCommand command = new(
            CreatorId: userId,
            ProjectId: projectId,
            Tasks: request.Tasks.Select(t => new AddProjectTaskDto(t.MembersLimit, t.Title, t.Description, t.CloseDate))
            );
        
        IEnumerable<ProjectTask> tasks = await handler.Handle(command, ct);
        return new Envelope(tasks.Select(t => t.ToDto()));
    }
    
    /// <summary>
    /// Добавление участников в проект
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="projectId">Идентификатор проекта</param>
    /// <param name="request">Запрос на добавление участников (Members)</param>
    /// <param name="handler">Обработчик добавления участников</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Добавленные участники</returns>
    [HttpPost("{id:guid}/members")]
    public async Task<Envelope> AddMembers(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromRoute(Name = "id")] Guid projectId,
        [FromBody] AddProjectMembersRequest request,
        [FromServices] AddProjectMembersHandler handler,
        CancellationToken ct
    )
    {
        AddProjectMembersCommand command = new(
            CreatorId: userId,
            ProjectId: projectId,
            MemberIds: request.Members.Select(m => m.Id)
        );

        IEnumerable<ProjectMember> members = await handler.Handle(command, ct);
        return new Envelope(members.Select(m => m.ToDto()));
    }
    
    /// <summary>
    /// Создание назначения участника к задаче
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="projectId">Идентификатор проекта</param>
    /// <param name="taskId">Идентификатор задачи</param>
    /// <param name="request">Запрос на создание назначения (MemberId)</param>
    /// <param name="handler">Обработчик создания назначения</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданное назначение</returns>
    [HttpPost("{id:guid}/tasks/{taskid:guid}/assignments")]
    public async Task<Envelope> MakeAssignment(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromRoute(Name = "id")] Guid projectId,
        [FromRoute(Name = "taskid")] Guid taskId,
        [FromBody] AssignMemberToTaskRequest request,
        [FromServices] AssignMemberToTaskHandler handler,
        CancellationToken ct
        )
    {
        AssignMemberToTaskCommand command = new(
            AssignerId: userId,
            ProjectId: projectId,
            TaskId: taskId,
            MemberId: request.MemberId
            );
        
        ProjectTaskAssignment assignment = await handler.Handle(command, ct);
        return new Envelope(assignment.ToDto());
    }
    
    /// <summary>
    /// Закрытие задачи
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="projectId">Идентификатор проекта</param>
    /// <param name="taskId">Идентификатор задачи</param>
    /// <param name="handler">Обработчик закрытия задачи</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Закрытая задача</returns>
    [HttpPatch("{id:guid}/tasks/{taskid:guid}/close")]
    public async Task<Envelope> CloseTask(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromRoute(Name = "id")] Guid projectId,
        [FromRoute(Name = "taskid")] Guid taskId,
        [FromServices] CloseProjectTaskHandler handler,
        CancellationToken ct
        )
    {
        CloseProjectTaskCommand command = new(
            CloserId: userId,
            ProjectId: projectId,
            TaskId: taskId
            );
        
        ProjectTask task = await handler.Handle(command, ct);
        return new Envelope(task.ToDto());
    }

    [HttpPost("{id:guid}")]
    public async Task<Envelope> UpdateInformation(
        [FromHeader(Name = "user-id")] Guid userId,
        [FromRoute(Name = "id")] Guid projectId,
        [FromQuery(Name = "name")] string? name,
        [FromQuery(Name = "description")] string? description,
        [FromServices] UpdateProjectInfoHandler handler,
        CancellationToken ct
        )
    {
        UpdateProjectInfoCommand command = new(
            CreatorId: userId,
            ProjectId: projectId,
            NewName: name,
            NewDescription: description
            );
        
        Project project = await handler.Handle(command, ct);
        return new Envelope(project.ToDto());
    }
}