using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class ModeratorSoul : MemberSoul
{
    public ModeratorSoul(string email, Guid spaceId, IRepositoryManager repositoryManager, IHelperManager helperManager)
        : base(email, spaceId, repositoryManager, helperManager)
    {
    }

    public async Task KickMemberAsync(string kickedByEmail, string memberEmail)
    {

    }
}
