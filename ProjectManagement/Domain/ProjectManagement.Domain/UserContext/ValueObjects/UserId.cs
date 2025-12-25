using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects;

/// <summary>
/// Идентификатор пользователя
/// </summary>
public readonly record struct UserId
{
    public UserId() => Value = Guid.NewGuid();

    private UserId(Guid value) => Value = value;

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Value { get; }

    public static UserId NewUserId()
    {
        return new UserId(Guid.NewGuid());
    }
    
    public static Result<UserId, Error> Create(Guid value)
    {
        ErrorResult<UserId> result = value switch
        {
            { } when value == Guid.Empty => Error.InvalidFormat("Идентификатор пользователя некорректный."),
            { } => new UserId(value),
        };
        return result;
    }
}
