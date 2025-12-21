namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Requests;

public sealed record AddProjectTasksRequest(IEnumerable<AddProjectTaskRequestBody> Tasks);