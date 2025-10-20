﻿using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Common;

public class BaseAuditableEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : class, IBaseAuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Created)
            .IsRequired(false);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(e => e.LastModified)
            .IsRequired(false);

        builder.Property(e => e.LastModifiedBy)
            .HasMaxLength(50)
            .IsRequired(false);
    }
}
