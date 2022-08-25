namespace Space.Services.Abstraction;

public interface ISoulService
{
    Task JoinSpaceAsync(Guid spaceId, string email);
}
