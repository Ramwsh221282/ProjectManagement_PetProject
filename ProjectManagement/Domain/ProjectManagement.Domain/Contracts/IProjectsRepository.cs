using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.Utilities;

namespace ProjectManagement.Domain.Contracts;

public interface IProjectsRepository
{
    Task<ProjectRegistrationApproval> GetApproval(ProjectName name, CancellationToken ct = default);
    Task Add(Project project, CancellationToken ct = default);
    Task<Result<Project, Nothing>> GetProject(Guid id, bool withLock = false, CancellationToken ct = default);
    Task<bool> Exists(ProjectTaskAssignment assignment, CancellationToken ct = default);
}