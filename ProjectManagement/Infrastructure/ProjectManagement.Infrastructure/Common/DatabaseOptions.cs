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
        if (string.IsNullOrEmpty(Host) || 
            string.IsNullOrEmpty(Port) || 
            string.IsNullOrEmpty(Password) || 
            string.IsNullOrEmpty(Database) || 
            string.IsNullOrEmpty(User))
            throw new InvalidOperationException("Не все параметры подключения к БД указаны.");
        
        return $"Host={Host};Port={Port};Username={User};Password={Password};Database={Database}";
    }
}