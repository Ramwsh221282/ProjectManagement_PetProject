namespace ProjectManagement.Presenters.Controllers.UsersContext.Dtos;

public sealed class UserDto
{
    public required Guid Id { get; set; }
    public required string Login { get; set; }
    public required string Phone { get; set; }
    public required DateTime Created { get; set; }
    public required string Status { get; set; }
}