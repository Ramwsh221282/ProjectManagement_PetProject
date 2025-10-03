namespace ProjectManagement.Domain.ProjectContexts.ValueObjects;

public sealed record ProjectName
{
    public const int MAX_PROJECT_NAME_LENGTH = 150;
    public string Value { get; }

    private ProjectName(string value)
    {
        Value = value;
    }

    public static ProjectName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Название проекта было пустым.");

        if (value.Length > MAX_PROJECT_NAME_LENGTH)
            throw new ArgumentException(
                $"Длина проекта превышает максимальную длину в {MAX_PROJECT_NAME_LENGTH} символов");

        return new ProjectName(value);
    }
}