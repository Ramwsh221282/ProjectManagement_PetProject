namespace ProjectManagement.Domain.UsersContext.ValueObjects;

public sealed record UserPhoneNumber
{
    public const int MAX_PHONE_NUMBER_LENGTH = 16;
    private UserPhoneNumber(string phone) => Phone = phone;
    
    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public string Phone { get; }
    

    public static UserPhoneNumber Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Номер телефона пользователя был пустым.");

        return phone.Length > MAX_PHONE_NUMBER_LENGTH
            ? throw new ArgumentException("Номер телефона невалиден.")
            : new UserPhoneNumber(phone);
    }
}