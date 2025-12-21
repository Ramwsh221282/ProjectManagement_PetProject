namespace ProjectManagement.Infrastructure.Common;

public sealed class DatabaseOptions
{
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;

    public string FormConnectionString()
    {
        return $"Host={Host};Port={Port};Username={User};Password={Password};Database={Database}";
    }
}