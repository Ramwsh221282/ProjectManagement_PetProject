namespace ProjectManagement.UseCases.Projects.CloseProjectTask;

public sealed record CloseProjectTaskCommand(
    Guid CloserId,
    Guid ProjectId,
    Guid TaskId
    );