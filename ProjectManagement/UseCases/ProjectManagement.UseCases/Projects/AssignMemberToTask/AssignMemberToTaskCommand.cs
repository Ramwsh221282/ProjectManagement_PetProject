namespace ProjectManagement.UseCases.Projects.AssignMemberToTask;

public sealed record AssignMemberToTaskCommand(
    Guid AssignerId,
    Guid ProjectId,
    Guid TaskId, 
    Guid MemberId);