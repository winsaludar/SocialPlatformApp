using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Channel : Entity
{
    public Channel(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public void SetName(string name) => Name = name;
}
