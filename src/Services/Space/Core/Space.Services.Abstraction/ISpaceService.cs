using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISpaceService
{
    Task<IEnumerable<SpaceDto>> GetAllAsync();
    Task KickSoulAsync(Guid spaceId, string email);
}
