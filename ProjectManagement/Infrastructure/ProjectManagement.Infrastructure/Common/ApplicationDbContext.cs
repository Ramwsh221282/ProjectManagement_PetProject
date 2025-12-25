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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
