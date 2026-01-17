using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.CreateProjectByUser;

public sealed class CreateProjectByUserHandler(
    IProjectsRepository projects,
    IUsersRepository users,
    IUnitOfWork unitOfWork,
    IValidator<CreateProjectByUserCommand> validator
)
{
    public async Task<Result<Project>> Handle(
        CreateProjectByUserCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<Project>();

        Result<User> user = await users.GetUser(command.UserId, ct);
        if (user.IsFailure)
            return user.OnError;

        Result<ProjectName> projectName = ProjectName.Create(command.ProjectName);
        Result<ProjectDescription> description = ProjectDescription.Create(
            command.ProjectDescription
        );

        ProjectRegistrationApproval approval = await projects.GetApproval(
            projectName.OnSuccess,
            ct
        );
        Result<Project> project = Project.CreateNew(
            projectName.OnSuccess,
            description.OnSuccess,
            user.OnSuccess,
            approval
        );
        if (project.IsFailure)
            return project.OnError;

        await projects.Add(project.OnSuccess, ct);

        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        return project;
    }
}
