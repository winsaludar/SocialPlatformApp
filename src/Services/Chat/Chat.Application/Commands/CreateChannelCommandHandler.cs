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
        Guid userId = await _userManager.GetUserIdByEmailAsync(request.CreatedBy);
        Guid channelId = request.TargetServer.AddChannel(Guid.NewGuid(), request.Name, userId, DateTime.UtcNow);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return channelId;
    }
}
