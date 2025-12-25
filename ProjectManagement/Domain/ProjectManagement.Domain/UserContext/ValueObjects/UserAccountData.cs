using System.Text.RegularExpressions;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects;

/// <summary>
/// Данные аккаунта пользователя
/// </summary>
public sealed partial record UserAccountData
{
    /// <summary>
    /// Регулярное выражение для проверки email
    /// </summary>
    private static readonly Regex _emailValidationRegex = EmailValidationRegex();

    /// <summary>
    /// Максимальная длина логина
    /// </summary>
    public const int MAX_LOGIN_LENGTH = 100;

    /// <summary>
    /// Минимальная длина логина
    /// </summary>
    public const int MIN_LOGIN_LENGTH = 5;

    private UserAccountData(string email, string login)
    {
        Email = email;
        Login = login;
    }
    
    public Result<UserAccountData, Error> ChangeEmail(string email)
    {
        return Create(email, Login);
    }

    public UserAccountData Copy()
    {
        return new UserAccountData(Email, Login);
    }
    
    public Result<UserAccountData, Error> ChangeLogin(string login)
    {
        return Create(Email, login);
    }
    
    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; }

    public static Result<UserAccountData, Error> Create(string email, string login)
    {
        ErrorResult<UserAccountData> result = (email, login) switch
        {
            { } when !_emailValidationRegex.IsMatch(email) => Error.InvalidFormat("Некорректный формат почты"),
            { } when string.IsNullOrWhiteSpace(email) => Error.Validation("Почта пользователя была пустой."),
            { } when string.IsNullOrWhiteSpace(login) => Error.Validation("Логин пользователя был пустым."),
            { } when login.Length > MAX_LOGIN_LENGTH => Error.Validation($"Логин пользователя превышает длину {MAX_LOGIN_LENGTH} символов"),
            { } when login.Length < MIN_LOGIN_LENGTH => Error.Validation($"Логин пользователя менее {MIN_LOGIN_LENGTH} символов"),
            { } => new UserAccountData(email, login),
        };
        return result;
    }

    /// <summary>
    /// Регулярное выражение для проверки email
    /// </summary>
    [GeneratedRegex(
        @"\b([a-z]+)[@]([a-z]+)[.](com|ru)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    )]
    private static partial Regex EmailValidationRegex();
}
