﻿using Domain.Common.Constants;
using Domain.Entities;
using Infrastructure.Data.Configurations.Common;
using Infrastructure.Data.TableNames;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ToDoConfiguration : BaseAuditableEntityConfiguration<ToDoEntity>
{
    public override void Configure(EntityTypeBuilder<ToDoEntity> builder)
    {
        base.Configure(builder);
        
        builder.ToTable(DatabaseConstants.ToDoItemTable);

        builder.Property(t => t.Title)
            .HasMaxLength(DbConstraints.MaxToDoNameLength)
            .IsRequired(true);
        builder.Property(t => t.Note)
            .HasMaxLength(DbConstraints.MaxToDoNameLength)
            .IsRequired(false);
        builder.Property(t => t.Priority)
            .IsRequired(true);
        builder.Property(t => t.Reminder)
            .IsRequired(false);
    }
}
