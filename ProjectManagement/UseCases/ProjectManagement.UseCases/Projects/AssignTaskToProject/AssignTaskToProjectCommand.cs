namespace ProjectManagement.UseCases.Projects.AssignTaskToProject;

public sealed record AssignTaskToProjectCommand(string Name, Guid TaskId, Guid MemberId);