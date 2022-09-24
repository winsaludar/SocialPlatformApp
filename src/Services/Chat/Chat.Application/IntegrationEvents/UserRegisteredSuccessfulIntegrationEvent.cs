using EventBus.Core.Events;

namespace Chat.Application.IntegrationEvents;

public record UserRegisteredSuccessfulIntegrationEvent : IntegrationEvent
{
    public UserRegisteredSuccessfulIntegrationEvent(Guid userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }

    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
}
