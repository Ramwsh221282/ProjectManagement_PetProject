namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

/// <summary>
/// Онлайн статус пользователя
/// </summary>
public sealed class UserStatusOnline : UserStatuses
{
    public UserStatusOnline()
        : base(0, "Онлайн") { }

    public override bool CanOperate()
    {
        return true;
    }
}
