namespace Space.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedDateUtc { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDateUtc { get; set; }
}
