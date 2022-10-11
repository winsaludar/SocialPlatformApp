using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetServerQueryHandler : IRequestHandler<GetServerQuery, ServerDto?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetServerQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<ServerDto?> Handle(GetServerQuery request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.ServerId);
        if (server is null)
            return null;

        return new ServerDto
        {
            Id = server.Id,
            Name = server.Name,
            ShortDescription = server.ShortDescription,
            LongDescription = server.LongDescription,
            Thumbnail = server.Thumbnail
        };
    }
}
