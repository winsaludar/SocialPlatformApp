using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class AddChannelMemberCommandHandler : IRequestHandler<AddChannelMemberCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public AddChannelMemberCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(AddChannelMemberCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.AddChannelMember(request.ChannelId, request.UserId);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
