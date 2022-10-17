using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public DeleteServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        await _repositoryManager.ServerRepository.DeleteAsync(request.TargetServerId);

        return true;
    }
}
