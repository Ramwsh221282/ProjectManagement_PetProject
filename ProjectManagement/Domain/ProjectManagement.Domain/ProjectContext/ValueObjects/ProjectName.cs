using System.Diagnostics;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.ValueObjects;

/// <summary>
/// Название проекта
/// </summary>
public sealed record ProjectName
{
    /// <summary>
    /// Максимальная длина названия проекта
    /// </summary>
    public const int MAX_PROJECT_NAME_LENGTH = 150;

    /// <summary>
    /// Значение названия проекта
    /// </summary>
    public string Value { get; }

    private ProjectName(string value)
    {
        Value = value;
    }

    private ProjectName() { } // ef core

    public static Result<ProjectName, Error> Create(string value)
    {
        ErrorResult<ProjectName> result = value switch
        {
            { } when string.IsNullOrWhiteSpace(value) => Error.Validation("Название проекта было пустым."),
            { } when value.Length > MAX_PROJECT_NAME_LENGTH => Error.Validation($"Длина проекта превышает максимальную длину в {MAX_PROJECT_NAME_LENGTH} символов."),
            { } => new ProjectName(value),
            _ => throw new UnreachableException()
        };
        return result;
    }
}
