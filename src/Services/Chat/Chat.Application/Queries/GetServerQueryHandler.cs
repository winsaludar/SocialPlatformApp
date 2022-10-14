using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetServerQueryHandler : IRequestHandler<GetServerQuery, Server?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetServerQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Server?> Handle(GetServerQuery request, CancellationToken cancellationToken) =>
        await _repositoryManager.ServerRepository.GetByIdAsync(request.ServerId);
}
