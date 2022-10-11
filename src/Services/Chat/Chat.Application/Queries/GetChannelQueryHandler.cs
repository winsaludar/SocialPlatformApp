using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, ChannelDto?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetChannelQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<ChannelDto?> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.ServerId);
        if (server is null)
            return null;

        var channel = server.Channels.FirstOrDefault(x => x.Id == request.ChannelId);
        if (channel is null)
            return null;

        return new ChannelDto
        {
            Id = channel.Id,
            Name = channel.Name
        };
    }
}
