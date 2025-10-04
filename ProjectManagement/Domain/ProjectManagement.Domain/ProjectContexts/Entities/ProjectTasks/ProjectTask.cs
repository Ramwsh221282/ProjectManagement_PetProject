using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks.ValueObjects;

namespace ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks;

public sealed class ProjectTask
{
    private readonly List<ProjectTaskMemberInfo> _taskMembers = [];
    public ProjectTaskId Id { get; }
    public ProjectTaskMembersLimit Limit { get; }
    public ProjectTaskStatusInfo StatusInfo { get; }
    public ProjectTaskInfo Information { get; }
    public IReadOnlyList<ProjectTaskMemberInfo> TaskMembers => _taskMembers;

    public ProjectTask(
        ProjectTaskId id,
        ProjectTaskMembersLimit limit,
        ProjectTaskStatusInfo statusInfo,
        ProjectTaskInfo information,
        IEnumerable<ProjectTaskMemberInfo> taskMembers
    )
    {
        _taskMembers = taskMembers.ToList();
        Id = id;
        Limit = limit;
        Information = information;
        StatusInfo = statusInfo;
    }
}
