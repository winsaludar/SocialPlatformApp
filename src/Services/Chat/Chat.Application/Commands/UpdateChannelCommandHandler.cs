using Chat.Domain.Aggregates.ServerAggregate;
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
        Server targetServer = request.TargetServer;
        Guid userId = await _userManager.GetUserIdByEmailAsync(request.UpdatedBy);
        targetServer.UpdateChannel(request.TargetChannelId, request.Name, DateTime.UtcNow, userId);

        await _repositoryManager.ServerRepository.UpdateAsync(targetServer);

        return true;
    }
}
