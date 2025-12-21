namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

public sealed class ProjectDto
{
    public required Guid Id { get; set; }
    public required DateOnly CreatedAt { get; set; }
    public required DateOnly? FinishedAt { get; set; }
    public required bool IsClosed { get; set; }
    public required string Description { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<ProjectTaskDto> Tasks { get; set; }
}