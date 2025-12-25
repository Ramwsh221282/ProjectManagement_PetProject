using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.CreateProjectByUser;

public sealed class CreateProjectByUserHandler(
    IProjectsRepository projects,
    IUsersRepository users,
    IUnitOfWork unitOfWork
    )
{
    public async Task<Result<Project, Error>> Handle(CreateProjectByUserCommand command, CancellationToken ct = default)
    {
        Result<User, Nothing> user = await users.GetUser(command.UserId, ct);
        if (user.IsFailure) return Failure<Project, Error>(Error.NotFound("Пользователь не найден."));
            
        Result<ProjectName, Error> projectName = ProjectName.Create(command.ProjectName);
        if (projectName.IsFailure) return Failure<Project, Error>(projectName.OnError);
        
        Result<ProjectDescription, Error> description = ProjectDescription.Create(command.ProjectDescription);
        if (description.IsFailure) return Failure<Project, Error>(description.OnError);
        
        ProjectRegistrationApproval approval = await projects.GetApproval(projectName.OnSuccess, ct);
        Result<Project, Error> project = Project.CreateNew(projectName.OnSuccess, description.OnSuccess, user.OnSuccess, approval);
        if (project.IsFailure) return Failure<Project, Error>(project.OnError);

        await projects.Add(project.OnSuccess, ct);
        
        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<Project, Error>(saving.OnError);
        
        return project;
    }
}