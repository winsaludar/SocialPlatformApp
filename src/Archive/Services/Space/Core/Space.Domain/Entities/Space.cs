using Space.Domain.Helpers;

namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    private readonly IHelperManager? _helperManager;
    private string _name = default!;

    public Space() { }

    public Space(IHelperManager helperManager) => _helperManager = helperManager;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            if (_helperManager != null)
                Slug = _helperManager.SlugHelper.CreateSlug(value);
        }
    }
    public string Creator { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Thumbnail { get; set; }
    public IList<Soul> Members { get; set; } = new List<Soul>();
    public IList<Topic> Topics { get; set; } = new List<Topic>();
    public IList<Soul> Moderators { get; set; } = new List<Soul>();
}
