using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Server : Entity, IAggregateRoot
{
    private readonly List<Channel> _channels;

    public Server(string name, string shortDescription, string longDescription, string creatorEmail, string? thumbnail = "")
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        CreatorEmail = creatorEmail;
        Thumbnail = thumbnail;
        _channels = new List<Channel>();
    }

    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string CreatorEmail { get; private set; }
    public string? Thumbnail { get; private set; }
    public IReadOnlyCollection<Channel> Channels => _channels;

    public Guid AddChannel(string name)
    {
        Channel newChannel = new(name);
        newChannel.SetId(Guid.NewGuid());
        newChannel.SetDateCreated(DateTime.UtcNow);
        newChannel.SetCreatedById(CreatedById);

        _channels.Add(newChannel);

        return newChannel.Id;
    }
}
