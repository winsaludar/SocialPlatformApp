using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        Server targetServer = request.TargetServer;
        targetServer.UpdateChannel(request.TargetChannelId, request.Name, DateTime.UtcNow, request.UpdatedById);

        await _repositoryManager.ServerRepository.UpdateAsync(targetServer);

        return true;
    }
}
