namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

public sealed class UserStatusBanned : UserStatuses
{
    public UserStatusBanned() : base(2, "Забанен")
    {
    }

    public override bool CanOperate()
    {
        return false;
    }
}