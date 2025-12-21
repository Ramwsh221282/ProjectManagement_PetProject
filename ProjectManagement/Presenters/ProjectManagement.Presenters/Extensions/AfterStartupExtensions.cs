using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Common;

namespace ProjectManagement.Presenters.Extensions;

public static class AfterStartupExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        await using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
            
        }
    }
}