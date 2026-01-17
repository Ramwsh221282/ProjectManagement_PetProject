using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.CloseProjectTask;

public sealed class CloseProjectTaskHandler(
    IProjectsRepository repository,
    ITransactionSource transactionSource,
    IUnitOfWork unitOfWork,
    IValidator<CloseProjectTaskCommand> validator
)
{
    public async Task<Result<ProjectTask>> Handle(
        CloseProjectTaskCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<ProjectTask>();

        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);

        Result<Project> project = await repository.GetProject(
            command.ProjectId,
            withLock: true,
            ct
        );
        if (project.IsFailure)
            return Error.NotFound("Проект не найден.");

        Result<ProjectMember> closer = project.OnSuccess.FindMember(command.CloserId);
        if (closer.IsFailure)
            return Error.NotFound("Обладатель проекта не найден.");

        Result<ProjectTask> task = project.OnSuccess.FindTask(command.TaskId);
        if (task.IsFailure)
            return Error.NotFound("Задача не найдена.");

        Result<Unit> closing = project.OnSuccess.CloseTask(task.OnSuccess);
        if (closing.IsFailure)
            return closing.OnError;

        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        Result<Unit> commit = await scope.CommitAsync(ct);
        return commit.IsFailure ? commit.OnError : task.OnSuccess;
    }
}
