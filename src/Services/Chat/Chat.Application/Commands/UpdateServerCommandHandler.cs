using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Server updatedServer = new(request.Name, request.ShortDescription, request.LongDescription, server.CreatorEmail, request.Thumbnail);
        Guid updatorId = await GetUpdatorId(request.EditorEmail);
        updatedServer.SetId(server.Id);
        updatedServer.SetCreatedById(server.CreatedById);
        updatedServer.SetDateCreated(server.DateCreated);
        updatedServer.SetLastModifiedById(updatorId);

        await _repositoryManager.ServerRepository.UpdateAsync(updatedServer);
        return true;
    }

    private async Task<Guid> GetUpdatorId(string? email)
    {
        // There is a possibility that a user does not have any data in the database.
        // This happens when the integration event handler that registers the user throws an error.
        // We will return an empty Guid just to proceed, later we will handle all servers that does have empty createdById

        if (string.IsNullOrEmpty(email))
            return Guid.Empty;

        var result = await _repositoryManager.UserRepository.GetByEmailAsync(email);
        if (result is null)
            return Guid.Empty;

        return result.Id;
    }
}
