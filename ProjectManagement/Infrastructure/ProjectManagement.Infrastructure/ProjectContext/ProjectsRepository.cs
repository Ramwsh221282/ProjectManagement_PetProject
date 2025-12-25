using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProjectManagement.Domain.Contracts;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.Utilities;
using ProjectManagement.Infrastructure.Common;

namespace ProjectManagement.Infrastructure.ProjectContext;

public sealed class ProjectsRepository(ApplicationDbContext context) : IProjectsRepository
{
    private ApplicationDbContext Context { get; } = context;
    
    public async Task<ProjectRegistrationApproval> GetApproval(ProjectName name, CancellationToken ct = default)
    {
        bool exists = await Context.Projects.AnyAsync(p => p.Name == name, cancellationToken: ct);
        if (!exists) return new ProjectRegistrationApproval(true);
        return new ProjectRegistrationApproval(false);
    }
    
    public async Task Add(Project project, CancellationToken ct = default)
    {
        await Context.Projects.AddAsync(project, ct);
    }
    
    public async Task<Result<Project, Nothing>> GetProject(Guid id, bool withLock = false, CancellationToken ct = default)
    {
        Result<ProjectId, Error> idVo = ProjectId.Create(id);
        if (idVo.IsFailure) return Failure<Project, Nothing>(new Nothing());
        
        if (withLock) await BlockProjectRow(idVo.OnSuccess, ct);
        Project? project = await Context.Projects
            .Include(p => p.Tasks).ThenInclude(t => t.Assignments)
            .Include(p => p.Members).ThenInclude(m => m.Assignments)
            .Include(p => p.Ownership)
            .FirstOrDefaultAsync(p => p.Id == idVo.OnSuccess, ct);
        
        return project is null ? Failure<Project, Nothing>(new Nothing()) : Success<Project, Nothing>(project);
    }
    
    public async Task<bool> Exists(ProjectTaskAssignment assignment, CancellationToken ct = default)
    {
        Guid memberId = assignment.MemberId.Value;
        Guid taskId = assignment.TaskId.Value;
        NpgsqlParameter memberIdParam = new("@memberId", memberId);
        NpgsqlParameter taskIdParam = new("@taskId", taskId);
        AssignmentExistsResult result = await Context.Database.SqlQueryRaw<AssignmentExistsResult>(@"
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
                SELECT p.id as project_id
                FROM projects p
                    WHERE p.id = {primitiveId}
                    FOR UPDATE OF p              
          ";
        await Context.Database.ExecuteSqlInterpolatedAsync(sql, ct);
    }

    private sealed class AssignmentExistsResult
    {
        public bool Exists { get; set; }
    }
}