using Microsoft.EntityFrameworkCore;
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

        // Build one-to-many relationship with Topic
        builder.HasMany(x => x.Topics)
            .WithOne(y => y.Soul)
            .OnDelete(DeleteBehavior.SetNull);

        // Build one-to-many relationship with Comment
        builder.HasMany(x => x.Comments)
            .WithOne(y => y.Soul)
            .OnDelete(DeleteBehavior.SetNull);

        // Build many-to-many relationship with Topic as Voter
        builder.HasMany(x => x.TopicVotes)
            .WithMany(y => y.SoulVoters)
            .UsingEntity<SoulTopicVote>(x => x.ToTable(nameof(SoulTopicVote)));
    }
}
