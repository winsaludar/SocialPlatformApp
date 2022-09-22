using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, string>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateServerCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<string> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        Server newServer = new(request.Name, request.ShortDescription, request.LongDescription, request.Thumbnail);
        await _repositoryManager.ServerRepository.AddAsync(newServer);

        return newServer.Id.ToString();
    }
}

