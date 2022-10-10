using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IUserManager _userManager;

    public UpdateChannelCommandHandler(IRepositoryManager repositoryManager, IUserManager userManager)
    {
        _repositoryManager = repositoryManager;
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Guid userId = await _userManager.GetUserIdByEmailAsync(request.UpdatedBy);
        server.UpdateChannel(request.TargetChannelId, request.Name, DateTime.UtcNow, userId);

        await _repositoryManager.ServerRepository.UpdateAsync(server);

        return true;
    }
}
