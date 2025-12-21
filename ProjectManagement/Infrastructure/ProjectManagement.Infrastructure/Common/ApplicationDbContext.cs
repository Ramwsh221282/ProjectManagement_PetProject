using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Infrastructure.Common;

/// <summary>
/// контекст для работы с БД в приложении
/// </summary>
public sealed class ApplicationDbContext(IOptions<DatabaseOptions> options) : DbContext
{
    private DatabaseOptions Options { get; } = options.Value;
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Options.FormConnectionString());
        optionsBuilder.LogTo(Console.WriteLine);
    }
    
    /// <summary>
    /// применение конфигураций моделей для базы данных
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // собираем все конфигурации (то, что наследует IEntityTypeConfiguration<T>) и используем.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
