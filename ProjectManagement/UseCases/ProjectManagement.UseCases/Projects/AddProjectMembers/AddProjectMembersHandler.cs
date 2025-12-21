using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.UseCases.Projects.AddProjectMembers;

public sealed class AddProjectMembersHandler(
    IProjectsRepository projects, 
    IUsersRepository users, 
    ITransactionSource transactionSource, 
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<ProjectMember>> Handle(
        AddProjectMembersCommand command, 
        CancellationToken ct = default)
    {
        await using ITransactionScope scope = await transactionSource.BeginTransactionScope(ct);
        
        Project? project = await projects.GetProject(command.ProjectId, withLock: true, ct);
        if (project is null) throw new InvalidOperationException("Проект не найден.");
        
        ProjectMember creator = project.FindMember(command.CreatorId);
        if (!creator.IsOwning(project)) throw new InvalidOperationException("Участник не является владельцем проекта.");
        
        ProjectMember[] membersToAdd = [..await GetProjectMembers(command, ct)];
        project.AddMembers(membersToAdd);
        
        await unitOfWork.SaveChangesAsync(ct);
        await scope.CommitAsync(ct);
        return membersToAdd;
    }

    private async Task<IEnumerable<ProjectMember>> GetProjectMembers(AddProjectMembersCommand command, CancellationToken ct)
    {
        IEnumerable<User> usersToAdd = await users.GetUsers(command.MemberIds, withLock: true, ct);
        return usersToAdd.Select(TransformUserToProjectMember);
    }

    private static ProjectMember TransformUserToProjectMember(User user)
    {
        ProjectMemberId memberId = ProjectMemberId.Create(user.UserId.Value);
        ProjectMemberLogin memberLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        return ProjectMember.CreateNewContributor(memberId, memberLogin);
    }
}