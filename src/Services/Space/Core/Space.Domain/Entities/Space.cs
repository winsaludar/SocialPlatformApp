using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    private readonly IRepositoryManager? _repositoryManager;
    private readonly IHelperManager? _helperManager;
    private string _name = default!;

    public Space() { }

    public Space(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public Space(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

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
