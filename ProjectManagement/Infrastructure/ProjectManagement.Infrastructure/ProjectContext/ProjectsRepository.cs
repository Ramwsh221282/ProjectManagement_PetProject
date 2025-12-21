using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Infrastructure.Common;

namespace ProjectManagement.Infrastructure.ProjectContext;

public sealed class ProjectsRepository(ApplicationDbContext context) : IProjectsRepository
{
    public async Task<ProjectRegistrationApproval> CheckProjectNameUniqueness(ProjectName name, CancellationToken ct = default)
    {
        bool exists = await context.Projects.AnyAsync(p => p.Name == name, cancellationToken: ct);
        if (!exists) return new ProjectRegistrationApproval(true);
        return new ProjectRegistrationApproval(false);
    }

    public async Task Add(Project project, CancellationToken ct = default)
    {
        await context.Projects.AddAsync(project, ct);
    }

    public async Task<Project?> GetProject(Guid id, bool withLock = false, CancellationToken ct = default)
    {
        if (!withLock)
        {
            return await context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Members)
                .Include(p => p.Ownership)
                .FirstOrDefaultAsync(p => p.Id == ProjectId.Create(id), ct);
        }

        FormattableString sql = $@"
                SELECT p.*, t.*, m.*, o.*
                FROM projects p
                LEFT JOIN project_tasks t ON p.id = t.project_id
                LEFT JOIN project_members m ON p.id = m.project_id
                LEFT JOIN project_ownerships o ON p.id = o.project_id
                WHERE p.id = {id}
                FOR UPDATE OF p, t
          ";
        
        return await context.Projects.FromSqlInterpolated(sql)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .Include(p => p.Ownership)
            .FirstOrDefaultAsync(ct);
    }
}