using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
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

    public async Task<Project?> GetProjectDemo(Guid id)
    {
        ProjectId idVo = ProjectId.Create(id);
        Project? project = await context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .Include(p => p.Ownership)
            .FirstOrDefaultAsync(p => p.Id == idVo);
        return project;
    }
    
    public async Task<Project?> GetProject(Guid id, bool withLock = false, CancellationToken ct = default)
    {
        ProjectId idVo = ProjectId.Create(id);
        if (withLock) await BlockProjectRow(idVo, ct);
        return await context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .Include(p => p.Ownership)
            .FirstOrDefaultAsync(p => p.Id == idVo, ct);
    }

    public async Task<bool> Exists(ProjectTaskAssignment assignment, CancellationToken ct = default)
    {
        Guid memberId = assignment.MemberId.Value;
        Guid taskId = assignment.TaskId.Value;
        NpgsqlParameter memberIdParam = new("@memberId", memberId);
        NpgsqlParameter taskIdParam = new("@taskId", taskId);
        AssignmentExistsResult result = await context.Database.SqlQueryRaw<AssignmentExistsResult>(@"
        SELECT EXISTS(SELECT 1 FROM project_task_assigmnets p 
                                WHERE p.member_id = @memberId AND p.task_id = @taskId
                                ) as ""Exists""
         ", memberIdParam, taskIdParam).FirstAsync(ct);
        return result.Exists;
    }

    private async Task BlockProjectRow(ProjectId id, CancellationToken ct)
    {
        Guid primitiveId = id.Value;
        FormattableString sql = $@"
                SELECT p.id as project_id, t.id as task_id, m.member_id as member_id, o.owner_id as owner_id
                FROM projects p
                INNER JOIN project_tasks t ON p.id = t.project_id
                INNER JOIN project_members m ON p.id = m.project_id
                INNER JOIN project_ownerships o ON p.id = o.project_id
                WHERE p.id = {primitiveId}
                FOR UPDATE OF p, t
          ";
        await context.Database.ExecuteSqlInterpolatedAsync(sql, ct);
    }

    private sealed class AssignmentExistsResult
    {
        public bool Exists { get; set; }
    }
}