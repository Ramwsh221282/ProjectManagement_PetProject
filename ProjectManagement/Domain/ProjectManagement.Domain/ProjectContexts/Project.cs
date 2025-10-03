using ProjectManagement.Domain.ProjectContexts.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContexts.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContexts.ValueObjects;

namespace ProjectManagement.Domain.ProjectContexts;

public sealed class Project
{
    private readonly List<ProjectMember> _members = [];
    private readonly List<ProjectTask> _tasks = [];
    public ProjectId Id { get; }
    public ProjectLifeTime LifeTime { get; }
    public ProjectDescription Description { get; }
    public ProjectName Name { get; }
    public IReadOnlyList<ProjectMember> Members => _members;
    public IReadOnlyList<ProjectTask> Tasks => _tasks;

    public Project(
        ProjectId id, 
        ProjectLifeTime lifeTime, 
        ProjectDescription description, 
        ProjectName name, 
        IEnumerable<ProjectMember> members, 
        IEnumerable<ProjectTask> tasks)
    {
        _members =  members.ToList();
        _tasks =  tasks.ToList();
        Id = id;
        LifeTime = lifeTime;
        Description = description;
        Name = name;
    }
}