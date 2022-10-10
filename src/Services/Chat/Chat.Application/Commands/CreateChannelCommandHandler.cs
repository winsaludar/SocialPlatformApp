using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IUserManager _userManager;

    public CreateChannelCommandHandler(IRepositoryManager repositoryManager, IUserManager userManager)
    {
        _repositoryManager = repositoryManager;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Guid userId = await _userManager.GetUserIdByEmailAsync(request.CreatedBy);
        Guid channelId = server.AddChannel(Guid.NewGuid(), request.Name, userId, DateTime.UtcNow);

        await _repositoryManager.ServerRepository.UpdateAsync(server);
        return channelId;
    }
}
