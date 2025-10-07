using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;

/// <summary>
/// Абстрактный класс образующий семейство статусов пользователей
/// </summary>
public abstract class UserStatuses : Enumeration<UserStatuses>
{
    public UserStatuses(int value, string name)
        : base(value, name) { }

    public abstract bool CanOperate();
}
