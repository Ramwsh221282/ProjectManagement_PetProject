using ProjectManagement.Domain.UserContext.ValueObjects;
using ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;

namespace ProjectManagement.Domain.UserContext;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User
{
    /// <summary>
    /// Данные пользовательского аккаунта
    /// </summary>
    public UserAccountData AccountData { get; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public UserId UserId { get; }

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public UserPhoneNumber PhoneNumber { get; }

    /// <summary>
    /// Дата регистрации пользователя
    /// </summary>
    public UserRegistrationDate RegistrationDate { get; }

    /// <summary>
    /// Статус пользователя
    /// </summary>
    public UserStatuses Status { get; }

    public User(
        UserId id,
        UserAccountData accountData,
        UserPhoneNumber phoneNumber,
        UserRegistrationDate registrationDate,
        UserStatuses status
    )
    {
        AccountData = accountData;
        UserId = id;
        AccountData = accountData;
        PhoneNumber = phoneNumber;
        RegistrationDate = registrationDate;
        Status = status;
    }
}
