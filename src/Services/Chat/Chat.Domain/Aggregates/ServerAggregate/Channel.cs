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

    public Guid AddMessage(Guid id, string username, string content)
    {
        Message message = new(username, content);
        message.SetId(id);

        _messages.Add(message);

        return message.Id;
    }
}
