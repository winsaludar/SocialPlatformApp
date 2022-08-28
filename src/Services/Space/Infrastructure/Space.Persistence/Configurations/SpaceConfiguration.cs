﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Configurations;

public class SpaceConfiguration : IEntityTypeConfiguration<DomainEntities.Space>
{
    public void Configure(EntityTypeBuilder<DomainEntities.Space> builder)
    {
        builder.ToTable(nameof(DomainEntities.Space));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.Creator).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.ShortDescription).IsRequired().HasMaxLength(200);
        builder.Property(x => x.LongDescription).IsRequired();

        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(36);
        builder.Property(x => x.CreatedDateUtc).IsRequired();

        // Build many-to-many relationship with Soul
        builder.HasMany(x => x.Souls)
            .WithMany(y => y.Spaces)
            .UsingEntity<DomainEntities.SpaceSoul>(x => x.ToTable(nameof(DomainEntities.SpaceSoul)));

        // Build one-to-many relationship with Topic
        builder.HasMany(x => x.Topics)
            .WithOne(y => y.Space)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
