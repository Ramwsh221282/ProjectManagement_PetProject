using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Ports;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.UseCases.Projects.ProjectCreation;

public sealed class CreateProjectCommandHandler
{
    private readonly IProjectsRepository _repository;

    public CreateProjectCommandHandler(IProjectsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Project> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        ProjectName projectName = ProjectName.Create(command.Name);
        Project? withExactName = await _repository.GetProject(command.Name, ct);
        if (withExactName != null)
            throw new InvalidOperationException("Project with that name already exists.");
        
        ProjectDescription projectDescription = ProjectDescription.Create(command.Description);
        Project project = new(projectDescription, projectName);
        await _repository.AddProject(project, ct);
        return project;
    }
}