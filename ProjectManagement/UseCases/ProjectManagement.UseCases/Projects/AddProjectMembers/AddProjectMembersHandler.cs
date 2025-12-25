using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.UseCases.Projects.AddProjectMembers;

public sealed class AddProjectMembersHandler(
    IProjectsRepository projects, 
    IUsersRepository users, 
    ITransactionSource transactionSource, 
    IUnitOfWork unitOfWork)
{
    public async Task<Result<IEnumerable<ProjectMember>, Error>> Handle(AddProjectMembersCommand command, CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);
        
        Result<Project, Nothing> project = await projects.GetProject(command.ProjectId, withLock: true, ct);
        if (project.IsFailure)
            return Failure<IEnumerable<ProjectMember>, Error>(Error.NotFound("Проект не найден."));
        
        Result<ProjectMember, Nothing> creator = project.OnSuccess.FindMember(command.CreatorId);
        if (creator.IsFailure) 
            return Failure<IEnumerable<ProjectMember>, Error>(Error.NotFound("Участник не найден."));
        
        if (!creator.OnSuccess.IsOwning(project.OnSuccess)) 
            return Failure<IEnumerable<ProjectMember>, Error>(Error.Conflict("Участник не является владельцем проекта."));
        
        Result<IEnumerable<ProjectMember>, Error> membersToAdd = await GetProjectMembers(command, ct);
        project.OnSuccess.AddMembers(membersToAdd.OnSuccess);
        
        Result<Unit, Error> saving = await unitOfWork.SaveChangesAsync(ct);
        if (saving.IsFailure) 
            return Failure<IEnumerable<ProjectMember>, Error>(saving.OnError);
        
        Result<Unit, Error> commit = await scope.CommitAsync(ct);
        return commit.IsFailure 
            ? Failure<IEnumerable<ProjectMember>, Error>(commit.OnError) 
            : Success<IEnumerable<ProjectMember>, Error>(membersToAdd.OnSuccess);
    }

    private async Task<Result<IEnumerable<ProjectMember>, Error>> GetProjectMembers(AddProjectMembersCommand command, CancellationToken ct)
    {
        IEnumerable<User> usersToAdd = await users.GetUsers(command.MemberIds, withLock: true, ct);
        Result<ProjectMember, Error>[] members = [..usersToAdd.Select(TransformUserToProjectMember)];
        Result<ProjectMember, Error>? failure = members.FirstOrDefault(v => v.IsFailure);
        if (failure is not null) return Failure<IEnumerable<ProjectMember>, Error>(failure.OnError);
        return Success<IEnumerable<ProjectMember>, Error>(members.Select(m => m.OnSuccess));
    }

    private static Result<ProjectMember, Error> TransformUserToProjectMember(User user)
    {
        Result<ProjectMemberId, Error> memberId = ProjectMemberId.Create(user.UserId.Value);
        if (memberId.IsFailure) return Failure<ProjectMember, Error>(memberId.OnError);
        
        Result<ProjectMemberLogin, Error> memberLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        if (memberLogin.IsFailure) return Failure<ProjectMember, Error>(memberLogin.OnError);
        
        ProjectMember member = ProjectMember.CreateNewContributor(memberId.OnSuccess, memberLogin.OnSuccess); 
        return Success<ProjectMember, Error>(member);
    }
}