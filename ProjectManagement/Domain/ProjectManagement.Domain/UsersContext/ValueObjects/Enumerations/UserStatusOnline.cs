namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

public sealed class UserStatusOnline : UserStatuses
{
    public UserStatusOnline() : base(0, "Онлайн")
    {
    }

    public override bool CanOperate()
    {
        return true;
    }
}