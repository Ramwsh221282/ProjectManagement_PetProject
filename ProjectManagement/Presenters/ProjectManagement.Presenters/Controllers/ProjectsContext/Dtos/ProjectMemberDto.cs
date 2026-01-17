namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

public sealed class ProjectMemberDto
{
    public required Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required string Login { get; set; }
    public required string Status { get; set; }
}