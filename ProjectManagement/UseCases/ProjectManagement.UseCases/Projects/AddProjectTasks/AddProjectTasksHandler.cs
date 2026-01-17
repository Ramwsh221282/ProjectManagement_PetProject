using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AddProjectTasks;

public sealed class AddProjectTasksHandler(
    IProjectsRepository repository,
    ITransactionSource transactionSource,
    IUnitOfWork unitOfWork,
    IValidator<AddProjectTasksCommand> validator
)
{
    public async Task<Result<IEnumerable<ProjectTask>>> Handle(
        AddProjectTasksCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ToError<IEnumerable<ProjectTask>>();

        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);

        Result<Project> project = await repository.GetProject(
            command.ProjectId,
            withLock: true,
            ct
        );
        if (project.IsFailure)
            return Error.NotFound("Проект не найден.");

        Result<ProjectMember> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure)
            return Error.NotFound("Участник не найден.");

        if (!creator.OnSuccess.IsOwning(project.OnSuccess))
            return Error.Conflict("Участник не является владельцем проекта.");

        ProjectTask[] tasks = ConvertProjectTasksFromDto(command.Tasks);

        project.OnSuccess.AddTasks(tasks);
        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        Result<Unit> commit = await scope.CommitAsync(ct);
        return commit.IsFailure ? commit.OnError : tasks;
    }

    private ProjectTask[] ConvertProjectTasksFromDto(IEnumerable<AddProjectTaskDto> tasks)
    {
        DateTime creationDate = DateTime.UtcNow;
        return tasks
            .Select(t =>
                ProjectTask.CreateNew(
                    membersLimit: ProjectTaskMembersLimit.Create(t.MembersLimit).OnSuccess,
                    information: ProjectTaskInfo.Create(t.Title, t.Description).OnSuccess,
                    schedule: ProjectTaskSchedule.Create(creationDate, t.CloseDate).OnSuccess
                )
            )
            .ToArray();
    }
}
