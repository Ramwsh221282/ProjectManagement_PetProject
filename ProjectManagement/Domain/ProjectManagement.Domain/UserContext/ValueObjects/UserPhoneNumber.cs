using System.Diagnostics;
using System.Text.RegularExpressions;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects;

public sealed record UserPhoneNumber
{
    private const RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    
    /// <summary>
    /// Регулярное выражение для проверки номера телефона
    /// </summary>
    private static readonly Regex[] _phoneValidationTemplates =
    [
        new(@"[+]\d\s{1}[(](\d{3})[)]\s{1}(\d{3})\s{1}(\d{2})-(\d{2})", OPTIONS),
        new(@"[+]\d(\d{3})(\d{3})(\d{2})(\d{2})", OPTIONS),
        new(@"^\d(\d{3})(\d{3})(\d{2})(\d{2})\b", OPTIONS),
    ];

    /// <summary>
    /// Максимальная длина номера телефона
    /// </summary>
    public const int MAX_PHONE_NUMBER_LENGTH = 18;

    private UserPhoneNumber(string phone) => Phone = phone;

    private UserPhoneNumber() { } // ef core

    /// <summary>
    /// Номер телефона пользователя
    /// </summary>
    public string Phone { get; }

    public static Result<UserPhoneNumber, Error> Create(string phone)
    {
        ErrorResult<UserPhoneNumber> result = phone switch
        {
            { } when string.IsNullOrWhiteSpace(phone) => Error.Validation("Номер телефона был пустым."),
            { } when phone.Length > MAX_PHONE_NUMBER_LENGTH => Error.Validation($"Номер телефона превышает длину {MAX_PHONE_NUMBER_LENGTH} символов."),
            { } when !IsPhoneNumberMatchesTemplate(_phoneValidationTemplates, phone) => Error.InvalidFormat("Номер телефона некорректного формата."),
            { } => new UserPhoneNumber(phone),
            _ => throw new UnreachableException()
        };
        return result;
    }

    private static bool IsPhoneNumberMatchesTemplate(IEnumerable<Regex> templates, string input)
    {
        foreach (var regex in templates)
        {
            if (regex.IsMatch(input))
                return true;
        }

        return false;
    }
}
