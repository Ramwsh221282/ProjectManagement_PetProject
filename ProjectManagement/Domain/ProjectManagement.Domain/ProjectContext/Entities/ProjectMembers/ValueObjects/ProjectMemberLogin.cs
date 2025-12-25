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

    private ProjectMemberLogin() { } // ef core

    public string Value { get; }

    public static Result<ProjectMemberLogin, Error> Create(string value)
    {
        Func<Result<ProjectMemberLogin, Error>> operation = value switch
        {
            { } v when string.IsNullOrWhiteSpace(v) => () =>
                Failure<ProjectMemberLogin, Error>(Error.InvalidFormat("Логин участника проекта был пустым.")),
            { } v when v.Length > MAX_PROJECT_MEMBER_LOGIN_LENGTH => () =>
                Failure<ProjectMemberLogin, Error>(Error.InvalidFormat($"Длина логина участника проекта более {MAX_PROJECT_MEMBER_LOGIN_LENGTH} символов.")),
            { } v when v.Length < MIN_PROJECT_MEMBER_LOG_LENGTH => () =>
                Failure<ProjectMemberLogin, Error>(Error.InvalidFormat($"Длина логина участника проекта менее {MIN_PROJECT_MEMBER_LOG_LENGTH} символов.")),
            { } v => () => Success<ProjectMemberLogin, Error>(new ProjectMemberLogin(v)),
        };
        return operation();
    }
}
