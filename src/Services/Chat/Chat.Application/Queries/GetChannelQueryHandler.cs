using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, Channel?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetChannelQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Channel?> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.ServerId);
        if (server is null)
            return null;

        return server.Channels.FirstOrDefault(x => x.Id == request.ChannelId);
    }
}
