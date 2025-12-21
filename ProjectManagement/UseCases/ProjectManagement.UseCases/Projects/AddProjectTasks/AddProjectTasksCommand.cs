namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed record AddProjectTasksCommand(
    Guid CreatorId,
    Guid ProjectId,
    IEnumerable<AddProjectTaskDto> Tasks
    );