using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectTasks.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Infrastructure.ProjectContext.Configurations;

// конфигурация для задачи проекта
public sealed class ProjectTaskEntityConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        // таблица задачи проекта
        builder.ToTable("project_tasks");

        // ключ задач проекта
        builder.HasKey(x => x.Id).HasName("pk_project_tasks");

        // конфигурием ключ
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectTaskId.Create(fromDb).OnSuccess);

        // конфигурируем внешний ключ к проекту
        builder
            .Property(x => x.ProjectId)
            .HasColumnName("project_id")
            .IsRequired()
            .HasConversion(toDb => toDb!.Value.Value, fromDb => ProjectId.Create(fromDb).OnSuccess);

        // конфигурируем лимит участников задач
        builder
            .Property(x => x.Limit)
            .HasColumnName("members_limit")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectTaskMembersLimit.Create(fromDb).OnSuccess);

        // конфигурируем сложный объект, который состоит из других сложных объектов
        builder.ComplexProperty(
            x => x.StatusInfo,
            cpb =>
            {
                cpb.ComplexProperty(
                    s => s.Status,
                    statusBuilder =>
                    {
                        statusBuilder.Property(s => s.Name).HasColumnName("name").IsRequired();
                        statusBuilder.Property(s => s.Value).HasColumnName("value").IsRequired();
                    }
                );

                cpb.ComplexProperty(
                    s => s.Schedule,
                    scheduleBuilder =>
                    {
                        scheduleBuilder.Property(s => s.Created).HasColumnName("created_at");
                        scheduleBuilder
                            .Property(s => s.Closed)
                            .HasColumnName("closed")
                            .IsRequired(false);
                    }
                );
            }
        );

        builder.ComplexProperty(
            x => x.Information,
            cpb =>
            {
                cpb.Property(s => s.Description)
                    .HasColumnName("description")
                    .HasMaxLength(ProjectTaskInfo.MAX_DESCRIPTION_LENGTH)
                    .IsRequired();

                cpb.Property(s => s.Title)
                    .HasColumnName("title")
                    .HasMaxLength(ProjectTaskInfo.MAX_TITLE_LENGTH)
                    .IsRequired();
            }
        );

        // конфигурируем связи 1 задача много назначений к этой задаче
        builder
            .HasMany(t => t.Assignments)
            .WithOne(a => a.Task)
            .HasForeignKey(a => a.TaskId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
