using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class ChangeUsernameCommandHandler : IRequestHandler<ChangeUsernameCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public ChangeUsernameCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        request.TargetServer.ChangeUsername(request.UserId, request.NewUsername);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return true;
    }
}
