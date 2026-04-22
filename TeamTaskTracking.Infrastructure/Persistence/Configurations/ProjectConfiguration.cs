using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Domain.Projects;

namespace TeamTaskTracking.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.HasIndex(x => x.OwnerUserId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1500);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasMany(x => x.Tasks)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
