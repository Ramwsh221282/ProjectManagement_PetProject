namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

/// <summary>
/// Забаненный статус пользователя
/// </summary>
public sealed class UserStatusBanned : UserStatuses
{
    public UserStatusBanned()
        : base(2, "Забанен") { }

    public override bool CanOperate()
    {
        return false;
    }
}
