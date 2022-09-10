namespace Space.Domain.Helpers;

public interface ISlugHelper
{
    string CreateSlug(string text, bool hasUniqueId = false);
}
