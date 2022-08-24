using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISoulRepository
{
    Task<Soul?> GetByEmailAsync(string email);
    Task CreateAsync(Soul newSoul);
    Task<bool> IsMemberOfSpace(Guid soulId, Guid spaceId);
}
