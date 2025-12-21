namespace ProjectManagement.UseCases.Projects.AddProjectMembers;

public sealed record AddProjectMembersCommand(
    Guid CreatorId,
    Guid ProjectId,
    IEnumerable<Guid> MemberIds
);