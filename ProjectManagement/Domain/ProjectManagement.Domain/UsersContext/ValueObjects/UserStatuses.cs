namespace ProjectManagement.Domain.UsersContext.ValueObjects;

/// <summary>
/// Статусы пользователя
/// </summary>
public enum UserStatuses
{
    Offline = 0,
    Online = 1,
    Banned = 2,
}