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
        Server newServer = new(request.Name, request.ShortDescription, request.LongDescription, request.CreatorEmail, request.Thumbnail);
        newServer.SetCreatedById(request.CreatedById);

        request.Categories.ForEach(x => newServer.AddCategory(x));

        var newId = await _repositoryManager.ServerRepository.CreateAsync(newServer);

        return newId;
    }
}

