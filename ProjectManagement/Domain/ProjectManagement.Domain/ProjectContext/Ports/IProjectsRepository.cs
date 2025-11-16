namespace ProjectManagement.Domain.ProjectContext.Ports;

public interface IProjectsRepository
{
    Task AddProject(Project project, CancellationToken ct);
    Task<Project?> GetProject(Guid id, CancellationToken ct);
    Task<Project?> GetProject(string Name, CancellationToken ct);
    public async Task Save(CancellationToken  ct);
}