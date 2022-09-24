using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using Mapster;
using MediatR;

namespace Chat.Application.Queries;

public class GetServersQueryHandler : IRequestHandler<GetServersQuery, IEnumerable<ServerDto>>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetServersQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<IEnumerable<ServerDto>> Handle(GetServersQuery request, CancellationToken cancellationToken)
    {
        int skip = (request.Page - 1) * request.Size;

        var result = await _repositoryManager.ServerRepository.GetAllAsync(skip, request.Size);
        if (!result.Any())
            return Enumerable.Empty<ServerDto>();

        return result.Adapt<List<ServerDto>>();
    }
}
