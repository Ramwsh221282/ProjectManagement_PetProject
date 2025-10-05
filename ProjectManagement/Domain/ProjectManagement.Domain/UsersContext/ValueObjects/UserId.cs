﻿namespace ProjectManagement.Domain.UsersContext.ValueObjects;

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

    public static UserId Create(Guid value) =>
        value == Guid.Empty
            ? throw new ArgumentException("Идентификатор пользователя некорректный.")
            : new UserId(value);
}
