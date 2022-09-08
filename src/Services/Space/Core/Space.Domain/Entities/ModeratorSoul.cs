using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class ModeratorSoul : MemberSoul
{
    public ModeratorSoul(string email, Guid spaceId, IRepositoryManager repositoryManager, IHelperManager helperManager)
        : base(email, spaceId, repositoryManager, helperManager)
    {
    }

    public async Task KickMemberAsync(string memberEmail)
    {
        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        if (string.IsNullOrEmpty(memberEmail))
            throw new InvalidSoulException(memberEmail);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(SpaceId);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(SpaceId);
        }

        // Make sure kickedBy email is valid
        Soul? kickedBySoul = await _repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (kickedBySoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(Email);
        }

        // Make sure kickedBy email is a moderator
        bool isModerator = await _repositoryManager.SoulRepository.IsModeratorOfSpaceAsync(kickedBySoul.Id, targetSpace.Id);
        if (!isModerator)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new UnauthorizedAccessException($"'{Email}' is not authorize to kick a member");
        }

        // Make sure member email is valid
        Soul? memberSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(memberEmail);
        if (memberSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(memberEmail);
        }

        // Make sure member email is a member
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(memberSoul.Id, targetSpace.Id);
        if (!isMember)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(memberEmail, targetSpace.Name);
        }

        // Remove soul
        await _repositoryManager.SoulRepository.DeleteSpaceMemberAsync(memberSoul.Id, targetSpace.Id);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }
}
