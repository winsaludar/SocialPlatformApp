using EventBus.Core.Events;

namespace Chat.IntegrationEvents.Events;

public record UserRegisteredSuccessfulIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }


    public UserRegisteredSuccessfulIntegrationEvent(Guid userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}
