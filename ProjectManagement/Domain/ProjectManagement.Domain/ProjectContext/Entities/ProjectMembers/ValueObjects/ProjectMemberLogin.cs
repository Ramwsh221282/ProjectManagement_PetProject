using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;

/// <summary>
/// Логин участника проекта
/// </summary>
public sealed record ProjectMemberLogin
{
    /// <summary>
    /// Макс. длина логина участника проекта
    /// </summary>
    public const int MAX_PROJECT_MEMBER_LOGIN_LENGTH = 100;

    /// <summary>
    /// Мин. длина логина участника проекта
    /// </summary>
    public const int MIN_PROJECT_MEMBER_LOG_LENGTH = 5;

    private ProjectMemberLogin(string value) => Value = value;

    private ProjectMemberLogin() => Value = null!; // ef core

    public string Value { get; }

    public static Result<ProjectMemberLogin> Create(string value) =>
        value switch
        {
            { } v when string.IsNullOrWhiteSpace(v) => Error.InvalidFormat(
                "Логин участника проекта был пустым."
            ),
            { } v when v.Length > MAX_PROJECT_MEMBER_LOGIN_LENGTH => Error.InvalidFormat(
                $"Длина логина участника проекта более {MAX_PROJECT_MEMBER_LOGIN_LENGTH} символов."
            ),
            { } v when v.Length < MIN_PROJECT_MEMBER_LOG_LENGTH => Error.InvalidFormat(
                $"Длина логина участника проекта менее {MIN_PROJECT_MEMBER_LOG_LENGTH} символов."
            ),
            { } v => new ProjectMemberLogin(v),
        };
}
