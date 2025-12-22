using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;

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
    public static User CreateNew(
        UserAccountData accountData, 
        UserPhoneNumber phoneNumber, 
        UserRegistrationApproval approval)
    {
        CheckRegistrationApproved(approval);
        
        var registrationDate = UserRegistrationDate.CreateByCurrentDate();
        var status = UserStatuses.Online;
        var userId = UserId.NewUserId();
        
        return new User
        {
            AccountData = accountData,
            PhoneNumber = phoneNumber,
            RegistrationDate = registrationDate,
            Status = status,
            UserId = userId
        };
    }

    public static async Task<User> CreateNew(
        string email,
        string login,
        string phoneNumber,
        IUsersRepository users,
        CancellationToken ct = default
        )
    {
        var accountData = UserAccountData.Create(email, login);
        var phone = UserPhoneNumber.Create(phoneNumber);
        var registrationDate = UserRegistrationDate.CreateByCurrentDate();
        var status = UserStatuses.Online;
        var userId = UserId.NewUserId();
        
        var approval = await users.CheckRegistrationApproval(
            accountData.Email, 
            accountData.Login, 
            phone.Phone, ct);
        
        CheckRegistrationApproved(approval);
        User user = Create(userId, accountData, phone, status, registrationDate);
        await users.Add(user, ct);
        return user;
    }

    public async Task UpdateUserAccountData(
        IUsersRepository repository,
        string? email = null, 
        string? login = null)
    {
        UserAccountData cloned = AccountData.Copy();
        
        if (email is not null)
            cloned = cloned.ChangeEmail(email);
        if (login is not null)
            cloned = cloned.ChangeLogin(login);
        
        UserRegistrationApproval approval = await repository.CheckRegistrationApproval(
            cloned.Email, 
            cloned.Login, 
            PhoneNumber.Phone);
        
        if (!approval.HasUniqueEmail)
            throw new InvalidOperationException("Почта пользователя не уникальна");
        if (!approval.HasUniqueLogin)
            throw new InvalidOperationException("Логин пользователя не уникален");
        
        AccountData = cloned;
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
    
    private static void CheckRegistrationApproved(UserRegistrationApproval approval)
    {
        List<string> errors = [];
        if (!approval.HasUniqueLogin)
            errors.Add("Логин пользователя не уникален");
        if (!approval.HasUniqueEmail)
            errors.Add("Почта пользователя не уникальна");
        if (!approval.HasUniquePhone)
            errors.Add("Номер телефона пользователя не уникален");
        if (errors.Count > 0)
            throw new InvalidOperationException(string.Join(", ", errors));
    }
}
