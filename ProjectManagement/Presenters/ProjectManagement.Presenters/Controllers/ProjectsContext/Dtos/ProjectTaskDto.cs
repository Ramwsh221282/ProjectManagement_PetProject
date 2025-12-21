namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

public sealed class ProjectTaskDto
{
    public required Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required int MembersLimit { get; set; }
    public required string StatusName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? FinishedAt { get; set; }
}