using FluentValidation;
using FluentValidation.Results;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.UseCases.Common;

namespace ProjectManagement.UseCases.Projects.AddProjectMembers;

public sealed class AddProjectMembersHandler(
    IProjectsRepository projects,
    IUsersRepository users,
    ITransactionSource transactionSource,
    IValidator<AddProjectMembersCommand> validator,
    IUnitOfWork unitOfWork
)
{
    public async Task<Result<IEnumerable<ProjectMember>>> Handle(
        AddProjectMembersCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (validationResult.IsValid == false)
            return validationResult.ToError<IEnumerable<ProjectMember>>();

        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);

        Result<Project> project = await projects.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure)
            return Error.NotFound("Проект не найден.");

        Result<ProjectMember> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure)
            return Error.NotFound("Участник не найден.");

        if (!creator.OnSuccess.IsOwning(project.OnSuccess))
            return Error.Conflict("Участник не является владельцем проекта.");

        Result<IEnumerable<ProjectMember>> membersToAdd = await GetProjectMembers(command, ct);

        project.OnSuccess.AddMembers(membersToAdd.OnSuccess);

        Result saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure)
            return saving.OnError;

        Result<Unit> commit = await scope.CommitAsync(ct);
        return commit.IsFailure ? commit.OnError : Success(membersToAdd.OnSuccess);
    }

    private async Task<Result<IEnumerable<ProjectMember>>> GetProjectMembers(
        AddProjectMembersCommand command,
        CancellationToken ct
    )
    {
        IEnumerable<User> usersToAdd = await users.GetUsers(command.MemberIds, withLock: true, ct);
        Result<ProjectMember>[] members = [.. usersToAdd.Select(TransformUserToProjectMember)];
        Result<ProjectMember>? failure = members.FirstOrDefault(v => v.IsFailure);

        return failure is not null
            ? (Result<IEnumerable<ProjectMember>>)failure.OnError
            : Success(members.Select(m => m.OnSuccess));
    }

    private static Result<ProjectMember> TransformUserToProjectMember(User user)
    {
        Result<ProjectMemberId> memberId = ProjectMemberId.Create(user.UserId.Value);
        if (memberId.IsFailure)
            return memberId.OnError;

        Result<ProjectMemberLogin> memberLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        if (memberLogin.IsFailure)
            return memberLogin.OnError;

        return ProjectMember.CreateNewContributor(memberId.OnSuccess, memberLogin.OnSuccess);
    }
}
