using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public AddMessageCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        Channel? channel = request.TargetServer.Channels.FirstOrDefault(x => x.Id == request.TargetChannelId);
        if (channel is null)
            throw new ChannelNotFoundException(request.TargetChannelId.ToString());

        Guid id = Guid.NewGuid();
        channel.AddMessage(id, request.Username, request.Content);
        await _repositoryManager.ServerRepository.UpdateAsync(request.TargetServer);

        return id;
    }
}
