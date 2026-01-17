using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.ProjectContext;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;
using ProjectManagement.Domain.ProjectContext.ValueObjects;
using ProjectManagement.Infrastructure.Extensions;

namespace ProjectManagement.Infrastructure.ProjectContext.Configurations;

public sealed class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // задаем таблицу
        builder.ToTable("projects");
        
        // устанавливаем ключ
        builder.HasKey(x => x.Id).HasName("pk_projects");

        // конфигурирем работу с ключом
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectId.Create(fromDb).OnSuccess);

        // конфигурируем работу со свойствами
        // где свойства - кастомный класс из 1 поля
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(ProjectName.MAX_PROJECT_NAME_LENGTH)
            .HasConversion(toDb => toDb.Value, fromDb => ProjectName.Create(fromDb).OnSuccess);

        builder.HasIndex(x => x.Name).IsUnique();

        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(ProjectDescription.MAX_PROJECT_DESCRIPTION_LENGTH)
            .HasConversion(toDb => toDb.Value, fromDb => ProjectDescription.Create(fromDb).OnSuccess);

        // конфигурируем работу со свойствами
        // где свойства - сложный объект из нескольких полей
        builder.ComplexProperty(
            x => x.LifeTime,
            cpb =>
            {
                cpb.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired()
                    .HasConversion(toDb => toDb.ToUtc(), fromDb => fromDb.ToUtc());
                
                cpb.Property(x => x.FinishedAt).HasColumnName("finished_at").IsRequired(false)
                    .HasConversion(toDb => toDb.ToUtc(), fromDb => fromDb.ToUtc());
            }
        );
        
        // конфигурируем связь 1 ко многим
        // 1 проект = много задач
        builder
            .HasMany(x => x.Tasks)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // конфигурируем связь 1 ко многим
        // 1 проект = много участников проекта
        builder
            .HasMany(x => x.Members)
            .WithOne(m => m.Project)
            .HasForeignKey(m => m.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(p => p.Ownership)
            .WithOne()
            .HasForeignKey<ProjectOwnership>(o => o.ProjectId);
    }
}