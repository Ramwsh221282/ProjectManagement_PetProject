using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Domain.UserContext;

namespace ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;

/// <summary>
/// Владелец проекта
/// </summary>
public sealed class ProjectOwnership
{
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public ProjectId ProjectId { get; private set; }
    
    /// <summary>
    /// Идентификатор владельца проекта
    /// </summary>
    public ProjectOwnerId OwnerId { get; private set; } = null!;

    private ProjectOwnership() { } // ef core
    
    public ProjectOwnership(Project project, ProjectOwnerId ownerId)
    {
        ProjectId = project.Id;
        OwnerId = ownerId;
    }

    public ProjectOwnership(ProjectId projectId, ProjectOwnerId ownerId)
    {
        ProjectId = projectId;
        OwnerId = ownerId;
    }

    public ProjectOwnership(ProjectId projectId, User user)
    {
        ProjectId = projectId;
        OwnerId = ProjectOwnerId.Create(user.UserId.Value).OnSuccess;
    }

    public ProjectOwnership(Project project, ProjectMember member)
    {
        ProjectId = project.Id;
        OwnerId = ProjectOwnerId.Create(member.MemberId.Value).OnSuccess;
    }
    
    /// <summary>
    /// Проверить, является ли участник владельцем проекта
    /// </summary>
    /// <param name="member">Участник</param>
    /// <returns>Является ли участник владельцем проекта</returns>
    public bool IsOwner(ProjectMember member)
    {
        return OwnerId.Id == member.MemberId.Value;
    }

    /// <summary>
    /// Проверить, является ли пользователь владельцем проекта
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns>Является ли пользователь владельцем проекта</returns>
    public bool IsOwner(User user)
    {
        return OwnerId.Id == user.UserId.Value;
    }
}