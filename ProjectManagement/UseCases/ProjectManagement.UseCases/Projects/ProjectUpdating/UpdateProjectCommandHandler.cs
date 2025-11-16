using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Ports;

namespace ProjectManagement.UseCases.Projects.ProjectUpdating;

public sealed class UpdateProjectCommandHandler
{
    private readonly IProjectsRepository _repository;

    public UpdateProjectCommandHandler(IProjectsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _repository.GetProject(request.IdentityName, cancellationToken);
        if (project == null)
            throw new InvalidOperationException("Project not found.");
        
        project.Update(request.NewName, request.NewDescription);
        await _repository.Save(cancellationToken);
        return project;
    }
}