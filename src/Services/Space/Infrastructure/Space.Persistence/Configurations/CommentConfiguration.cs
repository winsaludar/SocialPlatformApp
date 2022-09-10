using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Space.Domain.Entities;

namespace Space.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(nameof(Comment));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.TopicId).IsRequired();
        builder.Property(x => x.SoulId);

        builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(36);
        builder.Property(x => x.CreatedDateUtc).IsRequired();
    }
}
