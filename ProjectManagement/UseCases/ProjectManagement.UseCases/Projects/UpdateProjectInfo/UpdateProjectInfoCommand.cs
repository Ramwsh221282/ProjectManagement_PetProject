namespace ProjectManagement.UseCases.Projects.UpdateProjectInfo;

public sealed record UpdateProjectInfoCommand(
    Guid CreatorId,
 
    Guid ProjectId,
    string? NewName = null,
    string? NewDescription = null
)
{
    public bool NothingToUpdate() => NewName is null && NewDescription is null;
}