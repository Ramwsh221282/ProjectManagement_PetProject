using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;

namespace ProjectManagement.UseCases.Projects.UpdateProjectInfo;

public sealed class UpdateProjectInfoHandler(IProjectsRepository repository)
{
    public async Task<Project> Handle(UpdateProjectInfoCommand command, CancellationToken ct = default)
    {
        Project? project = await repository.GetProject(command.ProjectId);
        if (project is null) throw new InvalidOperationException("Проект не найден.");
        
        ProjectMember creator = project.FindMember(command.CreatorId);
        if (!creator.IsOwning(project)) throw new InvalidOperationException("Участник не является владельцем проекта.");
        
        if (command.NothingToUpdate()) return project;
        
        project.Update(command.NewName, command.NewDescription);
        ProjectRegistrationApproval approval = await repository.CheckProjectNameUniqueness(project.Name, ct);
        
        if (!approval.HasUniqueName) throw new InvalidOperationException("Проект с таким названием уже существует.");
        await repository.Add(project, ct);
        return project;
    }
}