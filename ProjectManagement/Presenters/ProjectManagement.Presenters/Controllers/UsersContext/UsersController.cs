using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.Infrastructure.UserContext;
using ProjectManagement.Presenters.Controllers.ProjectsContext;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;
using ProjectManagement.UseCases.Users.ModifyAccountData;
using ProjectManagement.UseCases.Users.RegisterUser;
using ProjectManagement.UseCases.Users.RemoveUserProfile;

namespace ProjectManagement.Presenters.Controllers.UsersContext;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
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
        Result<User, Error> result = await handler.Handle(command, ct);
        return Envelope.FromResult(result, user => user.ToDto());
    }
    
    /// <summary>
    /// Изменение данных аккаунта пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="email">Новая почта</param>
    /// <param name="login">Новый логин</param>
    /// <param name="handler">Обработчик изменения данных аккаунта</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Измененный пользователь</returns>
    [HttpPut("{id:guid}")]
    public async Task<Envelope> ModifyAccountData(
        [FromRoute(Name = "id")] Guid userId,
        [FromQuery(Name = "email")] string? email,
        [FromQuery(Name = "login")] string? login,
        [FromServices] ModifyUserAccountDataHandler handler,
        CancellationToken ct
        )
    {
        ModifyUserAccountDataCommand command = new(userId, email, login);
        Result<User, Error> result = await handler.Handle(command, ct);
        return Envelope.FromResult(result, user => user.ToDto());
    }
    
    /// <summary>
    /// Удаление пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="handler">Обработчик удаления пользователя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Удаленный пользователь</returns>
    [HttpDelete("{id:guid}")]
    public async Task<Envelope> DeleteUser(
        [FromRoute(Name = "id")] Guid userId,
        [FromServices] RemoveUserProfileHandler handler,
        CancellationToken ct
        )
    {
        RemoveUserCommand command = new(userId);
        Result<User, Error> result = await handler.Handle(command, ct);
        return Envelope.FromResult(result, user => user.ToDto());
    }
}