using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.ProjectContext.Entities.ProjectOwnershipping;
using ProjectManagement.Domain.ProjectContext.ValueObjects;

namespace ProjectManagement.Infrastructure.ProjectContext.Configurations;

public sealed class ProjectOwnershipConfiguration : IEntityTypeConfiguration<ProjectOwnership>
{
    public void Configure(EntityTypeBuilder<ProjectOwnership> builder)
    {
        builder.ToTable("project_ownerships");

        builder.HasKey(x => x.ProjectId).HasName("pk_project_ownerships");
        
        builder.Property(x => x.ProjectId)
            .HasColumnName("project_id")
            .HasConversion(toDb => toDb.Value, fromDb => ProjectId.Create(fromDb).OnSuccess);        
        
        builder.Property(x => x.OwnerId).HasColumnName("owner_id")
            .HasConversion(toDb => toDb.Id, fromDb => ProjectOwnerId.Create(fromDb).OnSuccess);
    }
}