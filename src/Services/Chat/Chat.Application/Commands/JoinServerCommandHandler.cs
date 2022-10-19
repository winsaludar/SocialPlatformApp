using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class JoinServerCommandHandler : IRequestHandler<JoinServerCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public JoinServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(JoinServerCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.AddMember(request.UserId, request.Username, DateTime.UtcNow);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
