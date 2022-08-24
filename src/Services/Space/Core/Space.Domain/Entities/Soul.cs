namespace Space.Domain.Entities;

public class Soul : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<Space> Spaces { get; set; } = new List<Space>();
}
