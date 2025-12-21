namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;

public sealed record AddProjectTaskRequest(
    short MembersLimit, 
    string Status, 
    DateTime? FinishedAt,
    string Title,
    string Description);