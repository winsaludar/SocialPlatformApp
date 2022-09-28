using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Guid channelId = server.AddChannel(request.Name);

        await _repositoryManager.ServerRepository.UpdateAsync(server);
        return channelId;
    }
}
