namespace ProjectManagement.Domain.UserContext.ValueObjects;

public readonly record struct UserRegistrationDate
{
    public UserRegistrationDate() => Value = DateOnly.FromDateTime(DateTime.UtcNow);

    private UserRegistrationDate(DateOnly value) => Value = value;

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateOnly Value { get; }

    public static UserRegistrationDate Create(DateOnly value)
    {
        if (value == DateOnly.MinValue)
            throw new ArgumentException("Дата регистрации пользователя некорректна");

        return value == DateOnly.MaxValue
            ? throw new ArgumentException("Дата регистрации пользователя некорректна")
            : new UserRegistrationDate(value);
    }
}
