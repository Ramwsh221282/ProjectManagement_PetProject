namespace ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

public sealed record AddProjectUserDto(Guid UserId, string Email, string Login, string Phone);