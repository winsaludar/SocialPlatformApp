using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class RemoveModeratorCommandHandler : IRequestHandler<RemoveModeratorCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public RemoveModeratorCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(RemoveModeratorCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.RemoveModerator(request.UserId);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
