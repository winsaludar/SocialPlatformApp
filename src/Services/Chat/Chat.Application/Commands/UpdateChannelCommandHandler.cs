using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        Server targetServer = request.TargetServer;
        Channel? updatedChannel = targetServer.UpdateChannel(request.TargetChannelId, request.Name, DateTime.UtcNow, request.UpdatedById);

        // Add channel members
        if (request.IsPublic && updatedChannel is not null)
        {
            foreach (var member in request.TargetServer.Members.Where(x => x.UserId != targetServer.CreatedById))
                updatedChannel.AddMember(member.UserId);
        }

        await _repositoryManager.ServerRepository.UpdateAsync(targetServer);

        return true;
    }
}
