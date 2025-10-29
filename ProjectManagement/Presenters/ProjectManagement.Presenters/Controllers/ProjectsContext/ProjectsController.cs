using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMemberAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.UserContext;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

[ApiController]
[Route("api/projects")]
public class ProjectsController
{
    [HttpPost]
    public IResult Create([FromBody] CreateProjectRequest request)
    {
        try
        {
            var projectName = ProjectName.Create(request.Name);
            var projectDescription = ProjectDescription.Create(request.Description);
            var project = new Project(projectDescription, projectName);
            ProjectsStorage.Projects.Add(project.Id.Value, project);
            return new Envelope(project.ToDto());
        }
        catch(Exception ex)
        {
            return new Envelope(HttpStatusCode.BadRequest, ex.Message);
        }
    }

    [HttpGet("{name}")]
    public IResult Get([FromRoute] string name)
    {
        Project? project = ProjectsStorage.Projects.Values.FirstOrDefault(c => c.Name.Value == name);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Не найден проект с названием: {name}");
        
        return new Envelope(project.ToDto());
    }
    
    [HttpGet("{id:guid}")]
    public IResult Get([FromRoute] Guid id)
    {
        if (!ProjectsStorage.Projects.TryGetValue(id, out Project? projectEntity))
            return new Envelope(HttpStatusCode.NotFound, $"Не найден проект с ID: {id}");
        return new Envelope(projectEntity.ToDto());
    }

    [HttpPatch("{name}/close")]
    public IResult Close([FromRoute] string name)
    {
        Project? project = GetByName(name);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Не найден проект с названием: {name}");
        
        project.Close();
        ProjectsStorage.Projects[project.Id.Value] = project;
        return new Envelope(project.ToDto());
    }

    [HttpPut("{name}")]
    public IResult Update(
        [FromRoute(Name = "name")] string identityName,
        [FromQuery(Name = "name")] string name, 
        [FromQuery(Name = "description")] string description)
    {
        Project? project = GetByName(identityName);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Не найден проект с названием: {name}");
        
        project.Update(name, description);
        ProjectsStorage.Projects[project.Id.Value] = project;
        return new Envelope(project.ToDto());
    }

    [HttpPost("{name}/tasks")]
    public IResult AddTasks(
        [FromRoute(Name = "name")] string name,
        [FromBody] AddProjectTaskRequest request)
    {
        Project? project = GetByName(name);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Не найден проект с названием: {name}");
        
        ProjectTaskId id = ProjectTaskId.Create(Guid.NewGuid());
        ProjectTaskMembersLimit membersLimit = ProjectTaskMembersLimit.Create(request.MembersLimit);
        ProjectTaskStatusInfo status = new ProjectTaskStatusInfo(
            ProjectTaskStatus.FromName(request.Status),
            ProjectTaskSchedule.Create(DateTime.UtcNow, request.FinishedAt)
        );
        ProjectTaskInfo info = ProjectTaskInfo.Create(request.Title, request.Description);
        ProjectTask task = new ProjectTask(id, membersLimit, status, info, project, []);
        project.AddTask(task);
        ProjectsStorage.Projects[project.Id.Value] = project;
        return new Envelope(task.ToDto(project));
    }

    [HttpPost("{name}/members/{userId:guid}")]
    public IResult AddMember(
        [FromRoute(Name = "name")] string projectName,
        [FromRoute(Name = "userId")] Guid userId
        )
    {
        var project = GetByName(projectName);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Проект с названием: {projectName} не найден.");

        if (!UsersStorage.Users.TryGetValue(userId, out var user))
            return new Envelope(HttpStatusCode.NotFound, $"Пользователь с ID: {userId} не найден.");

        var projectMemberId = ProjectMemberId.Create(user.UserId.Value);
        var projectMemberLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        var projectMemberStatus = new ProjectMemberStatusContributor();

        var member = new ProjectMember(
            projectMemberId, 
            projectMemberLogin, 
            projectMemberStatus,
            project, []);

        project.AddMember(member);
        ProjectsStorage.Projects[project.Id.Value] = project;
        return new Envelope(member.ToDto(project));
    }
    
    [HttpPost("{name}/tasks/{taskId:guid}/assignment")]
    public IResult CreateAssignment(
        [FromRoute(Name = "name")] string projectName,
        [FromRoute(Name = "taskId")] Guid taskId,
        [FromQuery(Name = "memberId")] Guid memberId
    )
    {
        var project = GetByName(projectName);
        if (project == null)
            return new Envelope(HttpStatusCode.NotFound, $"Проект с названием: {projectName} не найден.");

        var task = project.Tasks.FirstOrDefault(t => t.Id.Value == taskId);
        if (task == null)
            return new Envelope(HttpStatusCode.NotFound, $"Задача проекта с ID: {taskId} не найдена.");

        var projectMember = project.Members.FirstOrDefault(m => m.MemberId.Value == memberId);
        if (projectMember == null)
            return new Envelope(HttpStatusCode.NotFound, $"Участник задачи с ID: {memberId} не найден в проекте.");

        var assignment = new ProjectTaskAssignment(task, projectMember);
        task.AddAssignment(assignment);
        ProjectsStorage.Projects[project.Id.Value] = project;
        return new Envelope(assignment.ToDto());
    }
    
    private Project? GetByName(string name)
    {
        Project? project = ProjectsStorage.Projects.Values.FirstOrDefault(c => c.Name.Value == name);
        return project;
    }
}

public sealed record AddProjectUserDto(Guid UserId, string Email, string Login, string Phone);

public static class ProjectsExtensions
{
    public static Envelope ToEnvelope(this object @object, HttpStatusCode code)
    {
        return new Envelope(code, @object, null);
    }
    
    public static Envelope ToEnvelopeError(this HttpStatusCode code, string message)
    {
        return new Envelope(code, null, message);
    }
    
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto()
        {
            Id = project.Id.Value,
            Name = project.Name.Value,
            Description = project.Description.Value,
            CreatedAt = project.LifeTime.CreatedAt,
            FinishedAt = project.LifeTime.FinishedAt,
            IsClosed = project.LifeTime.IsFinished,
            Tasks = project.Tasks.Select(t => t.ToDto(project))
        };
    }

    public static ProjectTaskDto ToDto(this ProjectTask task, Project project)
    {
        return new ProjectTaskDto()
        {
            Id = task.Id.Value,
            FinishedAt = task.StatusInfo.Schedule.Closed,
            CreatedAt = task.StatusInfo.Schedule.Created,
            MembersLimit = task.Limit.Value,
            ProjectId = project.Id.Value,
            StatusName = task.StatusInfo.Status.Name
        };
    }

    public static ProjectMemberDto ToDto(this ProjectMember member, Project project)
    {
        return new ProjectMemberDto()
        {
            Id = member.MemberId.Value,
            Login = member.Login.Value,
            ProjectId = project.Id.Value,
            Status = member.Status.Name
        };
    }

    public static ProjectTaskAssignmentDto ToDto(this ProjectTaskAssignment assignment)
    {
        return new ProjectTaskAssignmentDto()
        {
            MemberInfo = assignment.Member.ToDto(assignment.Member.Project),
            TaskInfo = assignment.Task.ToDto(assignment.Member.Project)
        };
    }
}

public sealed class ProjectMemberDto
{
    public required Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required string Login { get; set; }
    public required string Status { get; set; }
}

public sealed class ProjectDto
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? FinishedAt { get; set; }
    public required bool IsClosed { get; set; }
    public required string Description { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<ProjectTaskDto> Tasks { get; set; }
}

public sealed class ProjectTaskDto
{
    public required Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required int MembersLimit { get; set; }
    public required string StatusName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? FinishedAt { get; set; }
}

public sealed class ProjectTaskAssignmentDto
{
    public required ProjectMemberDto MemberInfo { get; set; }
    public required ProjectTaskDto TaskInfo { get; set; }
}



public sealed record AddProjectTaskRequest(
    short MembersLimit, 
    string Status, 
    DateTime? FinishedAt,
    string Title,
    string Description);

public sealed class CreateProjectRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public sealed class Envelope : IResult
{
    public int Status { get; }
    public object? Result { get; }
    public string? Error { get; }

    public Envelope(HttpStatusCode statusCode, object? result = null, string? error = null)
    {
        Status = (int)statusCode;
        Result = result;
        Error = error;
    }

    public Envelope(object result)
    {
        Status = 200;
        Error = null;
        Result = result;
    }

    public Envelope(HttpStatusCode statusCode, string error)
    {
        Status = (int)statusCode;
        Error = error;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = Status;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsJsonAsync(this);
    }
}