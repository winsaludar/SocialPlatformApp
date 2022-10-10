using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
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
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Server updatedServer = new(request.Name, request.ShortDescription, request.LongDescription, server.CreatorEmail, request.Thumbnail);
        Guid userId = await _userManager.GetUserIdByEmailAsync(request.EditorEmail);
        updatedServer.SetId(server.Id);
        updatedServer.SetCreatedById(server.CreatedById);
        updatedServer.SetDateCreated(server.DateCreated);
        updatedServer.SetLastModifiedById(userId);

        await _repositoryManager.ServerRepository.UpdateAsync(updatedServer);
        return true;
    }
}
