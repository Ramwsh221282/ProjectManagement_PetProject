using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Infrastructure.UserContext;

public static class UsersStorage
{
    public static Dictionary<Guid, User> Users = [];
}