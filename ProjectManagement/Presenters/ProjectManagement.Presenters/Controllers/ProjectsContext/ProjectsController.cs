using System.Net;
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
using ProjectManagement.UseCases.Projects.AddProjectMember;
using ProjectManagement.UseCases.Projects.AddProjectTask;
using ProjectManagement.UseCases.Projects.AssignTaskToProject;
using ProjectManagement.UseCases.Projects.ProjectClosing;
using ProjectManagement.UseCases.Projects.ProjectCreation;
using ProjectManagement.UseCases.Projects.ProjectUpdating;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

[ApiController]
[Route("api/projects")]
public class ProjectsController
{
    [HttpPost]
    public async Task<IResult> Create(
        [FromBody] CreateProjectRequest request,
        [FromServices] CreateProjectCommandHandler handler,
        CancellationToken ct)
    {
        CreateProjectCommand command = new(request.Name, request.Description);
        Project project = await handler.Handle(command, ct);
        ProjectDto dto = project.ToDto();
        return new Envelope(HttpStatusCode.OK, dto);
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
    public async Task<IResult> Close(
        [FromRoute] string name,
        [FromServices] CloseProjectCommandHandler handler,
        CancellationToken ct)
    {
        CloseProjectCommand command = new(name);
        Project project = await handler.Handle(command, ct);
        return new Envelope(project.ToDto());
    }

    [HttpPut("{name}")]
    public async Task<IResult> Update(
        [FromRoute(Name = "name")] string identityName,
        [FromQuery(Name = "newname")] string newname, 
        [FromQuery(Name = "description")] string description,
        [FromServices] UpdateProjectCommandHandler handler,
        CancellationToken ct = default)
    {
        UpdateProjectCommand command = new(identityName, newname, description);
        Project result = await handler.Handle(command, ct);
        return new Envelope(result.ToDto());
    }

    [HttpPost("{name}/tasks")]
    public async Task<IResult> AddTasks(
        [FromRoute(Name = "name")] string name,
        [FromBody] AddProjectTaskRequest request,
        [FromServices] AddProjectTaskCommandHandler handler,
        CancellationToken ct)
    {
        AddProjectTaskCommand command = new(
            name, 
            request.MembersLimit,
            request.Status, 
            request.FinishedAt,
            request.Title, 
            request.Description);
        ProjectTask result = await handler.Handle(command, ct);
        return new Envelope(result.ToDto(result.Project));
    }

    [HttpPost("{name}/members/{userId:guid}")]
    public async Task<IResult> AddMember(
        [FromRoute(Name = "name")] string projectName,
        [FromRoute(Name = "userId")] Guid userId,
        [FromServices] AddProjectMemberTaskCommandHandler handler,
        CancellationToken ct
        )
    {
        AddProjectMemberTaskCommand command = new(userId, projectName);
        ProjectMember result = await handler.Handle(command, ct);
        return new Envelope(result.ToDto(result.Project));
    }
    
    [HttpPost("{name}/tasks/{taskId:guid}/assignment")]
    public async Task<IResult> CreateAssignment(
        [FromRoute(Name = "name")] string name,
        [FromRoute(Name = "taskId")] Guid taskId,
        [FromRoute(Name = "memberId")] Guid memberId,
        [FromServices] AssignTaskToProjectCommandHandler handler,
        CancellationToken ct
    )
    {
        AssignTaskToProjectCommand command = new(name, taskId, memberId);
        ProjectTaskAssignment result = await handler.Handle(command, ct);
        return new Envelope(result.ToDto());
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