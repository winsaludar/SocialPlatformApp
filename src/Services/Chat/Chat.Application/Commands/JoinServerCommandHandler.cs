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

        // Add member to all public channels
        foreach (var channel in request.TargetServer.Channels)
        {
            if (channel.IsPublic)
                channel.AddMember(request.UserId);
        }

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
