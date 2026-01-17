using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.UpdateProjectInfo;

public sealed class UpdateProjectInfoHandler(
    IProjectsRepository repository,
    IUnitOfWork unitOfWork,
    ITransactionSource transactionSource,
    IValidator<UpdateProjectInfoCommand> validator
)
{
    public async Task<Result<Project>> Handle(
        UpdateProjectInfoCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<Project>();

        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);

        var project = await repository.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure)
            return project.OnError;

        Result<ProjectMember> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure)
            return creator.OnError;

        if (!creator.OnSuccess.IsOwning(project.OnSuccess))
            return Error.Conflict("Участник не является владельцем проекта.");

        if (command.NothingToUpdate())
            return project.OnSuccess;

        Result update = project.OnSuccess.Update(command.NewName, command.NewDescription);
        if (update.IsFailure)
            return update.OnError;

        ProjectRegistrationApproval approval = await repository.GetApproval(
            project.OnSuccess.Name,
            ct
        );
        if (!approval.HasUniqueName)
            Error.Conflict("Проект с таким названием уже существует.");

        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        Result commit = await scope.CommitAsync(ct);

        return commit.IsFailure ? commit.OnError : project.OnSuccess;
    }
}
