using ProjectManagement.Domain.Contracts;
using ProjectManagement.Infrastructure.Common;
using ProjectManagement.Infrastructure.ProjectContext;
using ProjectManagement.Infrastructure.UserContext;
using ProjectManagement.UseCases.Projects.AddProjectMembers;
using ProjectManagement.UseCases.Projects.AddProjectTasks;
using ProjectManagement.UseCases.Projects.CloseProjectTask;
using ProjectManagement.UseCases.Projects.CreateProjectByUser;
using ProjectManagement.UseCases.Projects.UpdateProjectInfo;
using ProjectManagement.UseCases.Users.ModifyAccountData;
using ProjectManagement.UseCases.Users.RegisterUser;
using ProjectManagement.UseCases.Users.RemoveUserProfile;

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
        services.AddScoped<ITransactionSource, TransactionSource>();
    }
    
    public static void RegisterProjectsUseCases(this IServiceCollection services)
    {
        services.AddScoped<CreateProjectByUserHandler>();
        services.AddScoped<AddProjectMembersHandler>();
        services.AddScoped<AddProjectTasksHandler>();
        services.AddScoped<CloseProjectTaskHandler>();
        services.AddScoped<CreateProjectByUserHandler>();
        services.AddScoped<UpdateProjectInfoHandler>();
    }
    
    public static void RegisterUsersUseCases(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<ModifyUserAccountDataHandler>();
        services.AddScoped<RemoveUserProfileHandler>();
    }
}