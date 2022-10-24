using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        Guid channelId = request.TargetServer.AddChannel(Guid.NewGuid(), request.Name, request.IsPublic, request.CreatedById, DateTime.UtcNow);
        Channel? newChannel = request.TargetServer.Channels.FirstOrDefault(x => x.Id == channelId);
        if (newChannel is null)
            throw new ChannelNotFoundException(channelId.ToString());

        // Add channel memberrs
        newChannel.AddMember(request.TargetServer.CreatedById);
        newChannel.AddMember(request.CreatedById);
        if (request.IsPublic)
        {
            foreach (var member in request.TargetServer.Members)
                newChannel.AddMember(member.UserId);
        }

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return channelId;
    }
}
