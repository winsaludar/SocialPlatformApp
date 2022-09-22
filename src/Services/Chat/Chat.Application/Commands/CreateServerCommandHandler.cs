using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        Server newServer = new(request.Name, request.ShortDescription, request.LongDescription, request.Thumbnail);
        Guid serverGuid = await _repositoryManager.ServerRepository.AddAsync(newServer);

        return serverGuid;
    }
}

