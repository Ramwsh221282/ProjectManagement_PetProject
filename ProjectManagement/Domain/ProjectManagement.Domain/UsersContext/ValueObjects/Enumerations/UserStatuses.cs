using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.UsersContext.ValueObjects.Enumerations;

public abstract class UserStatuses : Enumeration<UserStatuses>
{
    public UserStatuses(int value, string name) : base(value, name)
    { }

    public abstract bool CanOperate();
}