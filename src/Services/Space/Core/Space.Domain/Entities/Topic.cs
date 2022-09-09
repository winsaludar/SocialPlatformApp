using Space.Domain.Helpers;

namespace Space.Domain.Entities;

public class Topic : BaseEntity
{
    private readonly IHelperManager? _helperManager;
    private string _title = default!;

    public Topic() { }
    public Topic(IHelperManager helperManager) => _helperManager = helperManager;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (_helperManager != null)
                Slug = _helperManager.SlugHelper.CreateSlug(value);
        }
    }
    public string Content { get; set; } = default!;
    public string Slug { get; private set; } = default!;
    public Guid SpaceId { get; set; }
    public Guid? SoulId { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }

    // Navigation Properties
    public Space Space { get; set; } = default!;
    public Soul? Soul { get; set; } = default!;
}
