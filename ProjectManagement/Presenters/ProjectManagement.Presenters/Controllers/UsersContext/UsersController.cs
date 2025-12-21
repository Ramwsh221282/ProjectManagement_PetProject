using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;
using ProjectManagement.Infrastructure.UserContext;
using ProjectManagement.Presenters.Controllers.ProjectsContext;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;
using ProjectManagement.UseCases.Users.RegisterUser;

namespace ProjectManagement.Presenters.Controllers.UsersContext;

[ApiController]
[Route("api/users")]
public sealed class UsersController
{
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="email">Почта</param>
    /// <param name="login">Логин</param>
    /// <param name="phone">Телефон</param>
    /// <param name="handler">Обработчик регистрации пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный пользователь</returns>
    [HttpPost]
    public async Task<Envelope> CreateUser(
        [FromHeader(Name = "email")] string email,
        [FromHeader(Name = "login")] string login,
        [FromHeader(Name = "phone")] string phone,
        [FromServices] RegisterUserHandler handler,
        CancellationToken ct
        )
    {
        RegisterUserCommand command = new(Email: email, Login: login, Phone: phone);
        User user = await handler.Handle(command, ct);
        return new Envelope(user.ToDto());
    }
}