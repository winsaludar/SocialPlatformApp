using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using Mapster;
using MediatR;

namespace Chat.Application.Queries;

public class GetUserServersQueryHandler : IRequestHandler<GetUserServersQuery, IEnumerable<ServerDto>>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetUserServersQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<IEnumerable<ServerDto>> Handle(GetUserServersQuery request, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.UserRepository.GetUserServers(request.UserId);
        if (!result.Any())
            return Enumerable.Empty<ServerDto>();

        return result.Adapt<List<ServerDto>>();
    }
}
