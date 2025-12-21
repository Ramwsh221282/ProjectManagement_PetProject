namespace ProjectManagement.UseCases.Projects.CreateProjectByUser;

public record CreateProjectByUserCommand(
    Guid UserId,
    string ProjectName,
    string ProjectDescription
    );