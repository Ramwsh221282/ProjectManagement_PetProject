using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects.Enumerations;
using ProjectManagement.Domain.ProjectContext.Ports;
using ProjectManagement.Domain.UserContext;
using ProjectManagement.Domain.UserContext.Ports;

namespace ProjectManagement.UseCases.Projects.AddProjectMember;

public sealed class AddProjectMemberTaskCommandHandler
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddProjectMemberTaskCommandHandler(IProjectsRepository projectsRepository, IUsersRepository usersRepository)
    {
        _projectsRepository = projectsRepository;
        _usersRepository = usersRepository;
    }
    
    public async Task<ProjectMember> Handle(AddProjectMemberTaskCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _projectsRepository.GetProject(request.UserId, cancellationToken);
        if (project is null)
            throw new InvalidOperationException("Project was not found.");

        User? user = await _usersRepository.Get(request.UserId, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("User was not found.");
        
        ProjectMemberId projectMemberId = ProjectMemberId.Create(request.UserId);
        ProjectMemberLogin projectMemberLogin = ProjectMemberLogin.Create(user.AccountData.Login);
        ProjectMemberStatusContributor projectMemberStatus = new();

        ProjectMember member = new(
            projectMemberId, 
            projectMemberLogin, 
            projectMemberStatus,
            project, []);

        project.AddMember(member);
        await _projectsRepository.Save(cancellationToken);
        return member;
    }
}