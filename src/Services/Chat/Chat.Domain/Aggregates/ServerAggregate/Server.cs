using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Server : Entity, IAggregateRoot
{
    public Server(string name, string shortDescription, string longDescription, string? thumbnail)
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        Thumbnail = thumbnail;
    }

    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string? Thumbnail { get; private set; }
}
