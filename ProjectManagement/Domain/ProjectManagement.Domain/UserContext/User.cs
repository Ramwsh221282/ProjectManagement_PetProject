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
    private User() { } // ef core

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
    public static Result<User, Error> CreateNew(
        UserAccountData accountData, 
        UserPhoneNumber phoneNumber, 
        UserRegistrationApproval approval)
    {
        Result<Unit, Error> approved = CheckRegistrationApproved(approval);
        if (approved.IsFailure) return Failure<User, Error>(approved.OnError);
        
        var registrationDate = UserRegistrationDate.CreateByCurrentDate();
        var status = UserStatuses.Online;
        var userId = UserId.NewUserId();
        
        User user = new User
        {
            AccountData = accountData,
            PhoneNumber = phoneNumber,
            RegistrationDate = registrationDate,
            Status = status,
            UserId = userId
        };
        
        return Success<User, Error>(user);
    }

    public static async Task<Result<User, Error>> CreateNew(
        string email,
        string login,
        UserPhoneNumber phone,
        IUsersRepository users,
        CancellationToken ct = default
        )
    {
        Result<UserAccountData, Error> accountData = UserAccountData.Create(email, login);
        if (accountData.IsFailure) return Failure<User, Error>(accountData.OnError);
        
        UserRegistrationDate registrationDate = UserRegistrationDate.CreateByCurrentDate();
        UserStatuses status = UserStatuses.Online;
        UserId userId = UserId.NewUserId();
        
        var approval = await users.CheckRegistrationApproval(
            accountData.OnSuccess.Email, 
            accountData.OnSuccess.Login, 
            phone.Phone, ct);
        
        CheckRegistrationApproved(approval);
        User user = Create(userId, accountData.OnSuccess, phone, status, registrationDate);
        await users.Add(user, ct);
        return Success<User, Error>(user);
    }

    public async Task<Result<Unit, Error>> UpdateUserAccountData(
        IUsersRepository repository,
        string? email = null, 
        string? login = null)
    {
        UserAccountData cloned = AccountData.Copy();
        Func<Result<UserAccountData, Error>> operation = (email, login) switch
        {
            (null, null) => () => Success<UserAccountData, Error>(cloned),
            (not null, not null) => () => cloned.ChangeEmail(email).Continue(r => r.OnSuccess.ChangeLogin(login)),
            (not null, null) => () => cloned.ChangeEmail(email),
            (null, not null) => () => cloned.ChangeLogin(login),
        };
        
        Result<UserAccountData, Error> result = operation();
        if (result.IsFailure) return Failure<Unit, Error>(result.OnError);
        
        UserRegistrationApproval approval = await repository.CheckRegistrationApproval(
            result.OnSuccess.Email, 
            result.OnSuccess.Login, 
            PhoneNumber.Phone);

        (bool uniqueLogin, bool uniqueEmail) = (approval.HasUniqueLogin, approval.HasUniqueEmail);
        Func<Result<Unit, Error>> update = (uniqueLogin, uniqueEmail) switch
        {
            (true, true) => () => Success<Unit, Error>(Unit.Value),
            (false, true) => () => Failure<Unit, Error>(Error.Conflict("Почта пользователя не уникальна")),
            (true, false) => () => Failure<Unit, Error>(Error.Conflict("Логин пользователя не уникален")),
            _ => throw new UnreachableException(),
        };
        return update();
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
            RegistrationDate = registrationDate
        };
    }
    
    private static Result<Unit, Error> CheckRegistrationApproved(UserRegistrationApproval approval)
    {
        (bool uniqueLogin, bool uniqueEmail, bool uniquePhone) = (approval.HasUniqueLogin, approval.HasUniqueEmail, approval.HasUniquePhone);
        Func<Result<Unit, Error>> operation = (uniqueLogin, uniqueEmail, uniquePhone) switch
        {
            (true, true, true) => () => Success<Unit, Error>(Unit.Value),
            (false, true, true) => () => Failure<Unit, Error>(Error.Conflict("Почта пользователя не уникальна")),
            (true, false, true) => () => Failure<Unit, Error>(Error.Conflict("Логин пользователя не уникален")),
            (true, true, false) => () => Failure<Unit, Error>(Error.Conflict("Номер телефона пользователя не уникален")),
            _ => () => Failure<Unit, Error>(Error.Conflict("Логин, почта или номер телефона пользователя не уникальны")),
        };
        
        return operation();
    }
}
