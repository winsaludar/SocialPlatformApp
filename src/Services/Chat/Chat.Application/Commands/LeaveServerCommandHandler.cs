using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class LeaveServerCommandHandler : IRequestHandler<LeaveServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public LeaveServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(LeaveServerCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.RemoveMember(request.UserId);

        // Remove member on all channels
        foreach (var channel in request.TargetServer.Channels)
            channel.RemoveMember(request.UserId);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
