using System.Diagnostics;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User
{
    private User()
    {
        AccountData = null!; // ef core
        UserId = default; // ef core
        PhoneNumber = null!;
        RegistrationDate = default;
        Status = null!;
    } // ef core

    /// <summary>
    /// Данные пользовательского аккаунта
    /// </summary>
    public UserAccountData AccountData { get; private set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public UserPhoneNumber PhoneNumber { get; private set; }

    /// <summary>
    /// Дата регистрации пользователя
    /// </summary>
    public UserRegistrationDate RegistrationDate { get; private set; }

    /// <summary>
    /// Статус пользователя
    /// </summary>
    public UserStatuses Status { get; private set; }

    /// <summary>
    /// Создание нового пользователя
    /// </summary>
    /// <param name="accountData">Информация о пользовательском аккаунте</param>
    /// <param name="phoneNumber">Номер телефона пользователя</param>
    /// <param name="approval">Разрешение на регистрацию</param>
    /// <returns>Зарегистрированный пользователь</returns>
    /// <exception cref="InvalidOperationException">Ошибка регистрации пользователя (не уникальный логин, почта или номер телефона)</exception>
    public static Result<User> CreateNew(
        UserAccountData accountData,
        UserPhoneNumber phoneNumber,
        UserRegistrationApproval approval
    )
    {
        Result<Unit> approved = CheckRegistrationApproved(approval);
        if (approved.IsFailure)
            return approved.OnError;

        var registrationDate = UserRegistrationDate.CreateByCurrentDate();
        var status = UserStatuses.Online;
        var userId = UserId.NewUserId();

        return new User()
        {
            AccountData = accountData,
            PhoneNumber = phoneNumber,
            RegistrationDate = registrationDate,
            Status = status,
            UserId = userId,
        };
    }

    public static async Task<Result<User>> CreateNew(
        string email,
        string login,
        UserPhoneNumber phone,
        IUsersRepository users,
        CancellationToken ct = default
    )
    {
        Result<UserAccountData> accountData = UserAccountData.Create(email, login);
        if (accountData.IsFailure)
            return accountData.OnError;

        UserRegistrationDate registrationDate = UserRegistrationDate.CreateByCurrentDate();
        UserStatuses status = UserStatuses.Online;
        UserId userId = UserId.NewUserId();

        var approval = await users.CheckRegistrationApproval(
            accountData.OnSuccess.Email,
            accountData.OnSuccess.Login,
            phone.Phone,
            ct
        );

        CheckRegistrationApproved(approval);
        User user = Create(userId, accountData.OnSuccess, phone, status, registrationDate);
        await users.Add(user, ct);
        return user;
    }

    private static Result<UserAccountData> Update(
        UserAccountData cloned,
        string login,
        string email
    )
    {
        Result<UserAccountData> emailChange = cloned.ChangeEmail(email);
        if (emailChange.IsFailure)
            return emailChange.OnError;
        Result<UserAccountData> loginChange = emailChange.OnSuccess.ChangeLogin(login);
        if (loginChange.IsFailure)
            return loginChange.OnError;
        return loginChange.OnSuccess;
    }

    public async Task<Result<Unit>> UpdateUserAccountData(
        IUsersRepository repository,
        string? email = null,
        string? login = null
    )
    {
        UserAccountData cloned = AccountData.Copy();
        Result<UserAccountData> updated = (email, login) switch
        {
            (null, null) => cloned,
            (not null, not null) => Update(cloned, login, email),
            (not null, null) => cloned.ChangeEmail(email),
            (null, not null) => cloned.ChangeLogin(login),
        };

        if (updated.IsFailure)
            return updated.OnError;

        UserRegistrationApproval approval = await repository.CheckRegistrationApproval(
            updated.OnSuccess.Email,
            updated.OnSuccess.Login,
            PhoneNumber.Phone
        );

        (bool uniqueLogin, bool uniqueEmail) = (approval.HasUniqueLogin, approval.HasUniqueEmail);
        return (uniqueLogin, uniqueEmail) switch
        {
            (true, true) => Unit.Value,
            (false, true) => Error.Conflict("Почта пользователя не уникальна"),
            (true, false) => Error.Conflict("Логин пользователя не уникален"),
            _ => throw new UnreachableException(),
        };
    }

    private static User Create(
        UserId id,
        UserAccountData accountData,
        UserPhoneNumber phoneNumber,
        UserStatuses status,
        UserRegistrationDate registrationDate
    )
    {
        return new User()
        {
            UserId = id,
            AccountData = accountData,
            PhoneNumber = phoneNumber,
            Status = status,
            RegistrationDate = registrationDate,
        };
    }

    private static Result<Unit> CheckRegistrationApproved(UserRegistrationApproval approval)
    {
        (bool uniqueLogin, bool uniqueEmail, bool uniquePhone) = (
            approval.HasUniqueLogin,
            approval.HasUniqueEmail,
            approval.HasUniquePhone
        );

        return (uniqueLogin, uniqueEmail, uniquePhone) switch
        {
            (true, true, true) => Unit.Value,
            (false, true, true) => Error.Conflict("Почта пользователя не уникальна"),
            (true, false, true) => Error.Conflict("Логин пользователя не уникален"),
            (true, true, false) => Error.Conflict("Номер телефона пользователя не уникален"),
            _ => Error.Conflict("Логин, почта или номер телефона пользователя не уникальны"),
        };
    }
}
