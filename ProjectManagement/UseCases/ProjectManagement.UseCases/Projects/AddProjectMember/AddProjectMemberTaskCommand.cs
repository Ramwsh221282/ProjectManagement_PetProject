namespace ProjectManagement.UseCases.Projects.AddProjectMember;

public record AddProjectMemberTaskCommand(Guid UserId, string ProjectName);