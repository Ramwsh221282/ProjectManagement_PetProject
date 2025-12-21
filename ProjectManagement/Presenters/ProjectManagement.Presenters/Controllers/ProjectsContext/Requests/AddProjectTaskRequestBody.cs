namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;

public sealed record AddProjectTaskRequestBody(
    short MembersLimit,
    string Title,
    string Description,
    DateTime? CloseDate = null);