using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public DeleteChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        server.RemoveChannel(request.TargetChannelId);

        await _repositoryManager.ServerRepository.UpdateAsync(server);
        return true;
    }
}
