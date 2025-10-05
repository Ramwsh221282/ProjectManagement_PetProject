using System.Text.RegularExpressions;

namespace ProjectManagement.Domain.UsersContext.ValueObjects;

public sealed record UserPhoneNumber
{
    /// <summary>
    /// Регулярное выражение для проверки номера телефона
    /// </summary>
    private static readonly Regex _phoneNumberValidationRegex = new(
        @"[+]\d\s([(]\w{3}[)])(\s\w{3}\s)(\w{2}[-]\w{2})",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// Максимальная длина номера телефона
    /// </summary>
    public const int MAX_PHONE_NUMBER_LENGTH = 18;

    private UserPhoneNumber(string phone) => Phone = phone;

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public string Phone { get; }

    public static UserPhoneNumber Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Номер телефона был пустым.");

        if (phone.Length > MAX_PHONE_NUMBER_LENGTH)
            throw new ArgumentException("Номер телефона некорректного формата.");

        Match match = _phoneNumberValidationRegex.Match(phone);
        if (!match.Success)
            throw new ArgumentException("Номер телефона некорректного формата.");

        return new UserPhoneNumber(phone);
    }
}
