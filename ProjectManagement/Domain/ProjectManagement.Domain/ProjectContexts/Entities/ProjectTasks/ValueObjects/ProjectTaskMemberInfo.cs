namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

public sealed record ProjectTaskMemberInfo
{
    public const int MAX_MEMBER_LOGIN_LENGTH = 150;
    public const int MIN_MEMBER_LOGIN_LENGTH = 4;
    public Guid MemberId { get; }
    public string MemberEmail { get; }
    public string MemberLogin { get; }

    private ProjectTaskMemberInfo(Guid memberId, string memberEmail, string memberLogin)
    {
        MemberId = memberId;
        MemberEmail = memberEmail;
        MemberLogin = memberLogin;
    }

    public static ProjectTaskMemberInfo Create(Guid memberId, string memberEmail, string memberLogin)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Некорректный ID участника задачи.");

        if (string.IsNullOrWhiteSpace(memberEmail))
            throw new ArgumentException("Почта пользователя была пустой.");

        if (string.IsNullOrWhiteSpace(memberLogin))
            throw new ArgumentException("Логин пользователя был пустым.");

        if (memberLogin.Length > MAX_MEMBER_LOGIN_LENGTH)
            throw new ArgumentException($"Логин пользователя превышает длину {MAX_MEMBER_LOGIN_LENGTH} символов.");
        
        if (memberLogin.Length < MIN_MEMBER_LOGIN_LENGTH)
            throw new ArgumentException($"Логин пользователя меньше длины {MIN_MEMBER_LOGIN_LENGTH} символов.");
        
        return new ProjectTaskMemberInfo(memberId, memberEmail, memberLogin);
    }
}