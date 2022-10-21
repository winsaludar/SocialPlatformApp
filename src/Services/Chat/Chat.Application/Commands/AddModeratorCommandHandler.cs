using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class AddModeratorCommandHandler : IRequestHandler<AddModeratorCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public AddModeratorCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(AddModeratorCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.AddModerator(request.UserId, DateTime.UtcNow);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
