namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

/// <summary>
/// Оффлайн статус пользователя
/// </summary>
public sealed class UserStatusOffline : UserStatuses
{
    public UserStatusOffline()
        : base(1, "Оффлайн") { }

    public override bool CanOperate()
    {
        return false;
    }
}
