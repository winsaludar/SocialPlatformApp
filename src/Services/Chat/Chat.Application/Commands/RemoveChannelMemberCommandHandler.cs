using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class RemoveChannelMemberCommandHandler : IRequestHandler<RemoveChannelMemberCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public RemoveChannelMemberCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(RemoveChannelMemberCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.RemoveChannelMember(request.ChannelId, request.UserId);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
