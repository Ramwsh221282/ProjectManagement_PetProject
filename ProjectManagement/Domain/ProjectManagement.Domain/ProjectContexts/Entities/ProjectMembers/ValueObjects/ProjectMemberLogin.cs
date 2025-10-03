namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers.ValueObjects;

public sealed record ProjectMemberLogin
{
    public const int MAX_PROJECT_MEMBER_LOGIN_LENGTH = 100;
    public const int MIN_PROJECT_MEMBER_LOG_LENGTH = 5;
    
    private ProjectMemberLogin(string value) => Value = value;
    public string Value { get; }

    public static ProjectMemberLogin Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Логин участника проекта был пустым.");

        if (value.Length > MAX_PROJECT_MEMBER_LOGIN_LENGTH)
            throw new ArgumentException(
                $"Длина логина участника проекта более {MAX_PROJECT_MEMBER_LOGIN_LENGTH} символов.");

        if (value.Length < MIN_PROJECT_MEMBER_LOG_LENGTH)
            throw new ArgumentException($"Длина участника проекта менее {MIN_PROJECT_MEMBER_LOG_LENGTH} символов.");

        return new ProjectMemberLogin(value);
    }
}