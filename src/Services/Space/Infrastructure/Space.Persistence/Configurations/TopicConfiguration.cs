using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Space.Domain.Entities;

namespace Space.Persistence.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(nameof(Topic));

        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Slug);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.Property(x => x.SpaceId).IsRequired();
        builder.Property(x => x.SoulId);

        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(36);
        builder.Property(x => x.CreatedDateUtc).IsRequired();
    }
}
