using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Infrastructure.Common;

/// <summary>
/// контекст для работы с БД в приложении
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// применение конфигураций моделей для базы данных
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // собираем все конфигурации (то, что наследует IEntityTypeConfiguration<T>) и используем.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
