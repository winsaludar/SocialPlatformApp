using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        Channel newChannel = request.TargetServer.AddChannel(Guid.NewGuid(), request.Name, request.IsPublic, request.CreatedById, DateTime.UtcNow);

        // All server members are automatically a channel member if public
        if (request.IsPublic)
        {
            foreach (var member in request.TargetServer.Members.Where(x => x.UserId != request.TargetServer.CreatedById))
                newChannel.AddMember(member.UserId);
        }

        // Server creator and channel creator is also automatically a member of the channel
        newChannel.AddMember(request.TargetServer.CreatedById);
        newChannel.AddMember(request.CreatedById);

        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return newChannel.Id;
    }
}
