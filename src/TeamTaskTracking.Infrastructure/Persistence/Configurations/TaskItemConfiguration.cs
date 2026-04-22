using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Domain.Projects;

namespace TeamTaskTracking.Infrastructure.Persistence.Configurations;

internal class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");   

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1500);

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.Property(x => x.CreateAtUtc)
            .IsRequired();

        builder.Property(x => x.ProjectId)
            .IsRequired();  
    }
}
