using ProjectManagement.Domain.Contracts;
using ProjectManagement.Infrastructure.Common;
using ProjectManagement.Infrastructure.ProjectContext;
using ProjectManagement.Infrastructure.UserContext;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;
using ProjectManagement.UseCases.Users.RegisterUser;

namespace ProjectManagement.Presenters.Extensions;

public static class DependencyInjectionExtensions
{
    public static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));
        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
    
    public static void RegisterProjectsUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateProjectByUserHandler>();
    }
    
    public static void RegisterUsersUseCases(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
    }
}