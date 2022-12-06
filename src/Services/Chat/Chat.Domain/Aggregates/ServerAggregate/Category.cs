using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Category : Enumeration
{
    public static Category Gaming = new(nameof(Gaming), 1);
    public static Category Education = new(nameof(Education), 2);
    public static Category ScienceAndTechnology = new("Science & Technology", 3);
    public static Category Entertainment = new(nameof(Entertainment), 4);

    public Category(string name, int id)
        : base(name, id)
    {
    }
}
