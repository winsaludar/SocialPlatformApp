using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public DeleteChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        Server server = request.TargetServer;
        server.RemoveChannel(request.TargetChannelId);

        await _repositoryManager.ServerRepository.UpdateAsync(server);

        return true;
    }
}
