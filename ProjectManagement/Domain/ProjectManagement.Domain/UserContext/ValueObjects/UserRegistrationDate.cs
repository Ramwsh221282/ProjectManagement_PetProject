using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects;

public readonly record struct UserRegistrationDate
{
    public UserRegistrationDate() => Value = DateOnly.FromDateTime(DateTime.UtcNow);

    private UserRegistrationDate(DateOnly value) => Value = value;

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateOnly Value { get; }

    public static UserRegistrationDate CreateByCurrentDate() => new(DateOnly.FromDateTime(DateTime.UtcNow));
    
    public static Result<UserRegistrationDate, Error> Create(DateOnly value)
    {
        ErrorResult<UserRegistrationDate> result = value switch
        {
            { } when value == DateOnly.MinValue => Error.InvalidFormat("Дата регистрации пользователя некорректна"),
            { } when value == DateOnly.MaxValue => Error.InvalidFormat("Дата регистрации пользователя некорректна"),
            { } => new UserRegistrationDate(value),
        };
        return result;
    }
}
