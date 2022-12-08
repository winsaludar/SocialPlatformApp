using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using Mapster;
using MediatR;

namespace Chat.Application.Queries;

public class GetUserServerChannelsQueryHandler : IRequestHandler<GetUserServerChannelsQuery, IEnumerable<ChannelDto>>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetUserServerChannelsQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<IEnumerable<ChannelDto>> Handle(GetUserServerChannelsQuery request, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var channels = request.TargetServer.Channels.Where(x => x.IsMember(request.UserId));
            return channels.Adapt<IEnumerable<ChannelDto>>();
        });
    }
}
