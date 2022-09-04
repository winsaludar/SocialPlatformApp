using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISpaceService
{
    Task<IEnumerable<SpaceDto>> GetAllAsync();
    Task<SpaceDto?> GetByIdAsync(Guid spaceId);
    Task<SpaceDto?> GetBySlugAsync(string slug);

    Task<IEnumerable<SoulDto>> GetAllModeratorsAsync(Guid spaceId);
    Task<IEnumerable<SoulDto>> GetAllMembersAsync(Guid spaceId);
    Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId);

    Task CreateTopicAsync(TopicDto dto);
    Task UpdateTopicAsync(TopicDto dto);
    Task DeleteTopicAsync(TopicDto dto);

    Task KickMemberAsync(Guid spaceId, string kickedByEmail, string memberEmail);
}
