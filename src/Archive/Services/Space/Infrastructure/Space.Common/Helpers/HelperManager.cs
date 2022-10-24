using Space.Domain.Helpers;

namespace Space.Common.Helpers;

public class HelperManager : IHelperManager
{
    private readonly Lazy<ISlugHelper> _lazySlugHelper;

    public HelperManager()
    {
        _lazySlugHelper = new Lazy<ISlugHelper>(() => new SlugHelper());
    }

    public ISlugHelper SlugHelper => _lazySlugHelper.Value;
}
