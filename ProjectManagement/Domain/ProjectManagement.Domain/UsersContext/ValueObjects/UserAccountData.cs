namespace ProjectManagement.Domain.UsersContext.ValueObjects;

/// <summary>
/// Данные аккаунта пользователя
/// </summary>
public sealed record UserAccountData
{
    public const int MAX_LOGIN_LENGTH = 100;
    public const int MIN_LOGIN_LENGTH = 5;
    
    private UserAccountData(string email, string login)
    {
        Email = email;
        Login = login;
    }
    
    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; }
    
    /// <summary>
    /// Пароль
    /// </summary>
    public string Login { get; }

    public static UserAccountData Create(string email, string login)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Почта пользователя была пустой.");

        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Логин пользователя был пустым.");

        if (login.Length > MAX_LOGIN_LENGTH)
            throw new ArgumentException(
                $"Логин пользователя превышает длину {MAX_LOGIN_LENGTH} символов"
            );

        return login.Length < MIN_LOGIN_LENGTH
            ? throw new ArgumentException($"Логин пользователя менее {MIN_LOGIN_LENGTH} символов")
            : new UserAccountData(email, login);
    }
}