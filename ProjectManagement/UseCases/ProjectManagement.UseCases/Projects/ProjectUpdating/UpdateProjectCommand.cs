namespace ProjectManagement.UseCases.Projects.ProjectUpdating;

public sealed record UpdateProjectCommand(string IdentityName, string NewName, string NewDescription);