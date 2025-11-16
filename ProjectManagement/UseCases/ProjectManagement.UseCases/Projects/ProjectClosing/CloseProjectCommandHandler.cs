using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Ports;

namespace ProjectManagement.UseCases.Projects.ProjectClosing;

public sealed class CloseProjectCommandHandler
{
    private readonly IProjectsRepository _repository;

    public CloseProjectCommandHandler(IProjectsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Project> Handle(CloseProjectCommand command, CancellationToken ct)
    {
        Project? project = await _repository.GetProject(command.Name, ct);
        if (project == null)
            throw new InvalidOperationException("Project not found.");
        project.Close();
        return project;
    }
}