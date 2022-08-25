﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Space.Domain.Entities;

namespace Space.Persistence.Configurations;

public class SoulConfiguration : IEntityTypeConfiguration<Soul>
{
    public void Configure(EntityTypeBuilder<Soul> builder)
    {
        builder.ToTable(nameof(Soul));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(36);
        builder.Property(x => x.CreatedDateUtc).IsRequired();
    }
}