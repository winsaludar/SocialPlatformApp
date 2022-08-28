using Space.Domain.Helpers;

namespace Space.Common.Helpers;

public class SlugHelper : ISlugHelper
{
    private readonly Slugify.SlugHelper _slugHelper;

    public SlugHelper()
    {
        _slugHelper = new Slugify.SlugHelper();
    }

    public string CreateSlug(string text)
    {
        return _slugHelper.GenerateSlug(text);
    }
}
