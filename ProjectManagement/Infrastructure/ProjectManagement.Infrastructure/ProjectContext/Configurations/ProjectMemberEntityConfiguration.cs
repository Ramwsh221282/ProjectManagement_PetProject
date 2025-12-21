using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectMembers.ValueObjects;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Infrastructure.ProjectContext.Configurations;

/// <summary>
/// Конфигурация модели участник проекта
/// </summary>
public sealed class ProjectMemberEntityConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        // в таблицу project_members
        builder.ToTable("project_members");

        // с ключом memberId
        builder.HasKey(x => x.MemberId).HasName("pk_project_members");

        // конфигурирование ключа memberId
        builder
            .Property(x => x.MemberId)
            .HasColumnName("member_id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectMemberId.Create(fromDb));

        // конфигурирование ключа projectId
        builder
            .Property(x => x.ProjectId)
            .IsRequired()
            .HasColumnName("project_id")
            .HasConversion(toDb => toDb!.Value.Value, fromDb => ProjectId.Create(fromDb));

        // конфигурирование логина
        builder
            .Property(x => x.Login)
            .HasColumnName("member_login")
            .HasMaxLength(ProjectMemberLogin.MAX_PROJECT_MEMBER_LOGIN_LENGTH)
            .IsRequired()
            .HasConversion(toDb => toDb.Value, fromDb => ProjectMemberLogin.Create(fromDb));

        // индекс уникальности для логина
        builder.HasIndex(x => x.Login).IsUnique();

        // конфигурирование комплексного свойства статус участника
        builder.ComplexProperty(
            x => x.Status,
            statusBuilder =>
            {
                statusBuilder.Property(s => s.Name).IsRequired().HasColumnName("status_name");
                statusBuilder.Property(s => s.Value).IsRequired().HasColumnName("status_code");
            }
        );

        // связь 1 ко многим с назначениями на задачу
        builder
            .HasMany(x => x.Assignments)
            .WithOne(a => a.Member)
            .HasForeignKey(a => a.MemberId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
