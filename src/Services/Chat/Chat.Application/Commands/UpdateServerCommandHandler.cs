using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        Server originalServer = request.TargetServer;
        Server updatedServer = new(request.Name, request.ShortDescription, request.LongDescription, originalServer.CreatorEmail, request.Thumbnail);
        updatedServer.SetId(originalServer.Id);
        updatedServer.SetCreatedById(originalServer.CreatedById);
        updatedServer.SetDateCreated(originalServer.DateCreated);
        updatedServer.SetLastModifiedById(request.UpdatedById);

        await _repositoryManager.ServerRepository.UpdateAsync(updatedServer);

        return true;
    }
}
