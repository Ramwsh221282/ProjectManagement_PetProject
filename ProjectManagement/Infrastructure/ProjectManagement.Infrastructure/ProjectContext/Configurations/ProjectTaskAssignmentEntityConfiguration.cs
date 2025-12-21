using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTaskAssignments.ValueObjects;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;

namespace ProjectManagement.Infrastructure.ProjectContext.Configurations;

/// <summary>
/// Назначение задачи
/// </summary>
public sealed class ProjectTaskAssignmentEntityConfiguration
    : IEntityTypeConfiguration<ProjectTaskAssignment>
{
    public void Configure(EntityTypeBuilder<ProjectTaskAssignment> builder)
    {
        // в таблицу project_task_assignments
        builder.ToTable("project_task_assigmnets");

        // конфигурирования ключа (поскольку связь ко многим - ключ составной)
        builder.HasKey(x => new { x.TaskId, x.MemberId }).HasName("pk_project_task_assignment");

        // конфигурирование даты назначения
        builder
            .Property(x => x.AssignmentDate)
            .HasColumnName("assignment_date")
            .IsRequired()
            .HasConversion(
                toDb => toDb.AssignedAt,
                fromDb => ProjectTaskAssignmentDate.Create(fromDb)
            );

        // конфигурирование ид задачи
        builder
            .Property(x => x.TaskId)
            .HasColumnName("task_id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectTaskId.Create(fromDb));

        // конфигурирование ид участника
        builder
            .Property(x => x.MemberId)
            .HasColumnName("member_id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectMemberId.Create(fromDb));
    }
}
