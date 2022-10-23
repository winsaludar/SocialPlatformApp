using shortid;
using shortid.Configuration;
using Space.Domain.Helpers;

namespace Space.Common.Helpers;

public class SlugHelper : ISlugHelper
{
    private readonly Slugify.SlugHelper _slugHelper;

    public SlugHelper()
    {
        _slugHelper = new Slugify.SlugHelper();
    }

    public string CreateSlug(string text, bool hasUniqueId = false)
    {
        string slug = _slugHelper.GenerateSlug(text);
        if (!hasUniqueId)
            return slug;

        string uniqueId = ShortId.Generate(new GenerationOptions(useNumbers: true, useSpecialCharacters: false));
        return $"{slug}-{uniqueId}";
    }
}
