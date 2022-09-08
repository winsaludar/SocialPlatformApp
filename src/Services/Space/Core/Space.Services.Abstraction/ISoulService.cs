using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISoulService
{
    Task CreateSpaceAsync(SpaceDto dto);
    Task JoinSpaceAsync(Guid spaceId, string email);
    Task LeaveSpaceAsync(Guid spaceId, string email);
    Task<IEnumerable<TopicDto>> GetAllTopicsByIdAsync(Guid soulId);
    Task<IEnumerable<TopicDto>> GetAllTopicsByEmailAsync(string email);
    Task<IEnumerable<SpaceDto>> GetAllModeratedSpacesAsync(string email);
}
