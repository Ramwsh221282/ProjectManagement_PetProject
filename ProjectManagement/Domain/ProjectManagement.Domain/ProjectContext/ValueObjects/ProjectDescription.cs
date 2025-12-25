using System.Diagnostics;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.ProjectContext.ValueObjects;

/// <summary>
/// Описание проекта
/// </summary>
public sealed record ProjectDescription
{
    /// <summary>
    /// Максимальная длина описания проекта
    /// </summary>
    public const int MAX_PROJECT_DESCRIPTION_LENGTH = 500;

    /// <summary>
    /// Значение название длины проекта
    /// </summary>
    public string Value { get; }

    private ProjectDescription(string value)
    {
        Value = value;
    }

    private ProjectDescription()
    {
        // ef core
    }

    public static Result<ProjectDescription, Error> Create(string value)
    {
         ErrorResult<ProjectDescription> result = value switch
        {
            { } when string.IsNullOrWhiteSpace(value) => Error.Validation("Описание проекта было пустым."),
            { } when value.Length > MAX_PROJECT_DESCRIPTION_LENGTH => Error.Validation($"Длина проекта превышает длину в {MAX_PROJECT_DESCRIPTION_LENGTH} символов."),
            { } => new ProjectDescription(value),
            _ => throw new UnreachableException()
        };
        return result;
    }
}
