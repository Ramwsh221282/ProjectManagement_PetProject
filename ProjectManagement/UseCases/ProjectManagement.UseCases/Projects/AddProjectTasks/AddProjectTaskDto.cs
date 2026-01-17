namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed record AddProjectTaskDto(
    short MembersLimit,
    string Title,
    string Description,
    DateTime? CloseDate = null
);