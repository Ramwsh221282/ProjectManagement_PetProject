namespace ProjectManagement.Domain.ProjectContexts.ValueObjects;

public sealed record ProjectDescription
{
    public const int MAX_PROJECT_DESCRIPTION_LENGTH = 500;
    public string Value { get; }

    private ProjectDescription(string value)
    {
        Value = value;
    }

    public static ProjectDescription Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Описание проекта было пустым.");

        if (value.Length > MAX_PROJECT_DESCRIPTION_LENGTH)
            throw new ArgumentException(
                $"Длина проекта превышает длину в {MAX_PROJECT_DESCRIPTION_LENGTH} символов."
            );

        return new ProjectDescription(value);
    }
}
