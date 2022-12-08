using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Channel : Entity
{
    private readonly List<Message> _messages;
    private readonly List<Guid> _members;

    public Channel(string name, bool isPublic)
    {
        Name = name;
        IsPublic = isPublic;
        _messages = new List<Message>();
        _members = new List<Guid>();
    }

    public string Name { get; private set; }
    public bool IsPublic { get; set; }
    public IReadOnlyCollection<Message> Messages => _messages;
    public IReadOnlyCollection<Guid> Members => _members;

    public void SetName(string name) => Name = name;

    public void AddMember(Guid userId)
    {
        if (IsMember(userId))
            return;

        _members.Add(userId);
    }

    public void RemoveMember(Guid userId)
    {
        if (!IsMember(userId))
            return;

        _members.Remove(userId);
    }

    public bool IsMember(Guid userId) => _members.Any(x => x == userId);

}
