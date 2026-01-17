using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UserContext.ValueObjects.Enumerations;

/// <summary>
/// Абстрактный класс образующий семейство статусов пользователей
/// </summary>
public class UserStatuses : Enumeration<UserStatuses>
{
    public static UserStatuses Online => new UserStatusOnline();
    public static UserStatuses Offline => new UserStatusOffline();
    public static UserStatuses Banned => new UserStatusBanned();
    
    public UserStatuses(int value, string name)
        : base(value, name) { }
    
    private UserStatuses() : base(0, "") 
    { } // ef core

    public virtual bool CanOperate()
    {
        return false;
    }
}
