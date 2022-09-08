using EventBus.Core.Events;

namespace Space.IntegrationEvents.Events;

public record UserRegisteredSuccessfulIntegrationEvent : IntegrationEvent
{
    public string Username { get; init; }
    public string Email { get; init; }


    public UserRegisteredSuccessfulIntegrationEvent(string username, string email)
    {
        Username = username;
        Email = email;
    }
}
