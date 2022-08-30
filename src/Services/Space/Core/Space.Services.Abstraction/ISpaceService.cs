﻿using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISpaceService
{
    Task<IEnumerable<SpaceDto>> GetAllAsync();
    Task KickSoulAsync(Guid spaceId, string email);
    Task<IEnumerable<SoulDto>> GetAllSoulsAsync(Guid spaceId);
    Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId);
    Task CreateTopicAsync(TopicDto dto);
    Task UpdateTopicAsync(TopicDto dto);
}
