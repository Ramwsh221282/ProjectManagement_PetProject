namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

public sealed class ProjectTaskAssignmentDto
{
    public required Guid Id { get; set; }
    public required ProjectMemberDto MemberInfo { get; set; }
    public required ProjectTaskDto TaskInfo { get; set; }
}