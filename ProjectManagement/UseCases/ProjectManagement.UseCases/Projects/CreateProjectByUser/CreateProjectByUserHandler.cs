using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.UseCases.Projects.CreateProjectByUser;

public sealed class CreateProjectByUserHandler(
    IProjectsRepository projects,
    IUsersRepository users,
    IUnitOfWork unitOfWork
    )
{
    public async Task<Project> Handle(CreateProjectByUserCommand command, CancellationToken ct = default)
    {
        User? user = await users.GetUser(command.UserId, ct);
        if (user is null) throw new InvalidOperationException("Пользователь не найден.");

        ProjectName projectName = ProjectName.Create(command.ProjectName);
        ProjectDescription description = ProjectDescription.Create(command.ProjectDescription);
        ProjectRegistrationApproval approval = await projects.CheckProjectNameUniqueness(projectName, ct);

        Project project = Project.CreateNew(projectName, description, user, approval);

        await projects.Add(project, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return project;
    }
}