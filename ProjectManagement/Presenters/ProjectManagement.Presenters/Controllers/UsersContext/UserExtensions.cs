using ProjectManagement.Domain.UserContext;
using ProjectManagement.Presenters.Controllers.UsersContext.Dtos;

namespace ProjectManagement.Presenters.Controllers.UsersContext;

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