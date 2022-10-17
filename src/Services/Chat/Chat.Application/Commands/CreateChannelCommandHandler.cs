using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        Guid channelId = request.TargetServer.AddChannel(Guid.NewGuid(), request.Name, request.CreatedById, DateTime.UtcNow);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return channelId;
    }
}
