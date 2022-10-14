using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Channel : Entity
{
    private readonly List<Message> _messages;

    public Channel(string name)
    {
        Name = name;
        _messages = new List<Message>();
    }

    public string Name { get; private set; }
    public IReadOnlyCollection<Message> Messages => _messages;

    public void SetName(string name) => Name = name;
}
