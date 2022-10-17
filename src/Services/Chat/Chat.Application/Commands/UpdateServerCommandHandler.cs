using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IUserManager _userManager;

    public UpdateServerCommandHandler(IRepositoryManager repositoryManager, IUserManager userManager)
    {
        _repositoryManager = repositoryManager;
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        Server originalServer = request.TargetServer;
        Server updatedServer = new(request.Name, request.ShortDescription, request.LongDescription, originalServer.CreatorEmail, request.Thumbnail);
        Guid userId = await _userManager.GetUserIdByEmailAsync(request.EditorEmail);
        updatedServer.SetId(originalServer.Id);
        updatedServer.SetCreatedById(originalServer.CreatedById);
        updatedServer.SetDateCreated(originalServer.DateCreated);
        updatedServer.SetLastModifiedById(userId);

        await _repositoryManager.ServerRepository.UpdateAsync(updatedServer);

        return true;
    }
}
