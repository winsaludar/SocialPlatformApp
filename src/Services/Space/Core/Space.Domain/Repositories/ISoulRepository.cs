﻿using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISoulRepository
{
    Task<Soul?> GetByEmailAsync(string email, bool includeSpaces = false, bool includeTopics = false);
    Task<Soul?> GetByIdAsync(Guid id, bool includeSpaces = false, bool includeTopics = false);
    Task CreateAsync(Soul newSoul);
    Task<bool> IsMemberOfSpaceAsync(Guid soulId, Guid spaceId);
    Task DeleteSoulSpaceAsync(Guid soulId, Guid spaceId);
}
