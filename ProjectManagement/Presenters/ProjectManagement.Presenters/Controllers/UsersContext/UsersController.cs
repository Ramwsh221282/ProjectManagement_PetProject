using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;
using ProjectManagement.Infrastructure.UserContext;
using ProjectManagement.Presenters.Controllers.ProjectsContext;

namespace ProjectManagement.Presenters.Controllers.UsersContext;

[ApiController]
[Route("api/users")]
public sealed class UsersController
{
    [HttpPost]
    public IResult CreateUser(
        [FromHeader(Name = "email")] string email,
        [FromHeader(Name = "login")] string login,
        [FromHeader(Name = "phone")] string phone
        )
    {
        UserId userId = new UserId();
        UserAccountData accountData = UserAccountData.Create(email, login);
        UserPhoneNumber phoneNumber = UserPhoneNumber.Create(phone);
        UserRegistrationDate date = UserRegistrationDate.Create(DateOnly.FromDateTime(DateTime.UtcNow));
        var status = new UserStatusOnline();
        var user = new User(userId, accountData, phoneNumber, date, status);
        UsersStorage.Users.Add(user.UserId.Value, user);
        return new Envelope(user.ToDto());
    }
}

public sealed class UserDto
{
    public required Guid Id { get; set; }
    public required string Login { get; set; }
    public required string Phone { get; set; }
    public required DateTime Created { get; set; }
    public required string Status { get; set; }
}

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto()
        {
            Id = user.UserId.Value,
            Created = user.RegistrationDate.Value.ToDateTime(new TimeOnly()),
            Login = user.AccountData.Login,
            Phone = user.PhoneNumber.Phone,
            Status = user.Status.Name
        };
    }
}

