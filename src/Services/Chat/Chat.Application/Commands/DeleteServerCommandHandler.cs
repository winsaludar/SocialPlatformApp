using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public DeleteServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        await _repositoryManager.ServerRepository.DeleteAsync(request.TargetServerId);
        return true;
    }
}
