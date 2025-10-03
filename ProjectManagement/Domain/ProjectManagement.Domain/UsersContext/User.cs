using ProjectManagement.Domain.UsersContext.ValueObjects;

namespace ProjectManagement.Domain.UsersContext;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User
{
    public UserAccountData AccountData { get; }
    public UserId UserId { get; }
    public UserPhoneNumber PhoneNumber { get; }
    public UserRegistrationDate RegistrationDate { get; }
    
    public UserStatuses Status { get; }

    public User(
        UserId id, 
        UserAccountData accountData, 
        UserPhoneNumber phoneNumber, 
        UserRegistrationDate registrationDate, 
        UserStatuses status)
    {
        AccountData = accountData;
        UserId = id;
        AccountData = accountData;
        PhoneNumber = phoneNumber;
        RegistrationDate = registrationDate;
    }
}