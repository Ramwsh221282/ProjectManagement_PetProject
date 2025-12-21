namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;

public sealed class CreateProjectRequest
{
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}