using System.Net;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Presenters.Controllers.ProjectsContext.Dtos;

namespace ProjectManagement.Presenters.Controllers.ProjectsContext;

public static class ProjectsExtensions
{
    public static Envelope ToEnvelope(this object @object, HttpStatusCode code)
    {
        return new Envelope(code, @object, null);
    }
    
    public static Envelope ToEnvelopeError(this HttpStatusCode code, string message)
    {
        return new Envelope(code, null, message);
    }
    
    public static ProjectDto ToDto(this Project project)
    {
        ProjectMember owner = project.FindMember(project.Ownership.OwnerId.Id);
        return new ProjectDto()
        {
            Id = project.Id.Value,
            Name = project.Name.Value,
            Description = project.Description.Value,
            CreatedAt = project.LifeTime.CreatedAt,
            FinishedAt = project.LifeTime.FinishedAt,
            IsClosed = project.LifeTime.IsFinished,
            Tasks = project.Tasks.Select(t => t.ToDto()),
            Members = project.Members.Select(m => m.ToDto()),
            Owner = owner.ToDto()
        };
    }

    public static ProjectTaskDto ToDto(this ProjectTask task)
    {
        return new ProjectTaskDto()
        {
            Id = task.Id.Value,
            FinishedAt = task.StatusInfo.Schedule.Closed,
            CreatedAt = task.StatusInfo.Schedule.Created,
            MembersLimit = task.Limit.Value,
            ProjectId = task.ProjectId!.Value.Value,
            StatusName = task.StatusInfo.Status.Name
        };
    }

    public static ProjectMemberDto ToDto(this ProjectMember member)
    {
        return new ProjectMemberDto()
        {
            Id = member.MemberId.Value,
            Login = member.Login.Value,
            ProjectId = member.ProjectId!.Value.Value,
            Status = member.Status.Name
        };
    }

    public static ProjectTaskAssignmentDto ToDto(this ProjectTaskAssignment assignment)
    {
        return new ProjectTaskAssignmentDto()
        {
            Id = assignment.Id.Value,
            MemberInfo = assignment.Member.ToDto(),
            TaskInfo = assignment.Task.ToDto()
        };
    }
}