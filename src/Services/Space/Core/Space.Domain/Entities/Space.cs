namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    public string Creator { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string? Thumbnail { get; set; }
    public IList<Soul> Souls { get; set; } = new List<Soul>();
}
