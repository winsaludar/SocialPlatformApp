using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery, IEnumerable<ChannelDto>>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetChannelsQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<IEnumerable<ChannelDto>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (result is null)
            return Enumerable.Empty<ChannelDto>();

        List<ChannelDto> channels = new();
        foreach (var item in result.Channels)
        {
            channels.Add(new()
            {
                Id = item.Id,
                Name = item.Name,
                IsPublic = item.IsPublic,
                Members = item.Members.ToList()
            });
        }

        return channels;
    }
}
