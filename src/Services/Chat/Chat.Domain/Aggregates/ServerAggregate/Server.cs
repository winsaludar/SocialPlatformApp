using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Server : Entity, IAggregateRoot
{
    private readonly List<Channel> _channels;
    private readonly List<Member> _members;

    public Server(string name, string shortDescription, string longDescription, string creatorEmail, string? thumbnail = "")
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        CreatorEmail = creatorEmail;
        Thumbnail = thumbnail;
        _channels = new List<Channel>();
        _members = new List<Member>();
    }

    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string CreatorEmail { get; private set; }
    public string? Thumbnail { get; private set; }
    public IReadOnlyCollection<Channel> Channels => _channels;
    public IReadOnlyCollection<Member> Members => _members;

    public Guid AddChannel(Guid id, string name, Guid createdById, DateTime dateCreated, Guid? lastModifiedById = null, DateTime? dateLastModified = null)
    {
        Channel newChannel = new(name);
        newChannel.SetId(id);
        newChannel.SetDateCreated(dateCreated);
        newChannel.SetCreatedById(CreatedById);

        if (lastModifiedById.HasValue)
            newChannel.SetLastModifiedById(lastModifiedById.Value);

        if (dateLastModified.HasValue)
            newChannel.SetDateLastModified(dateLastModified.Value);

        _channels.Add(newChannel);

        return newChannel.Id;
    }

    public void UpdateChannel(Guid channelId, string name, DateTime dateLastModified, Guid lastModifiedId)
    {
        var channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return;

        channel.SetName(name);
        channel.SetDateLastModified(dateLastModified);
        channel.SetLastModifiedById(lastModifiedId);
    }

    public void RemoveChannel(Guid channelId)
    {
        var channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return;

        _channels.Remove(channel);
    }

    public void AddMember(Guid userId, string username, DateTime dateJoined) => _members.Add(new(userId, username, dateJoined));

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(x => x.UserId == userId);
        if (member is null)
            return;

        _members.Remove(member);
    }
}
