using System.Text.Json.Serialization;

namespace EventBus.Core.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDateUtc = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdDateUtc)
    {
        Id = id;
        CreatedDateUtc = createdDateUtc;
    }

    [JsonInclude]
    public Guid Id { get; private init; }

    [JsonInclude]
    public DateTime CreatedDateUtc { get; private init; }
}
