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
        Guid creatorId = await GetCreatorId(request.CreatorEmail);
        newServer.SetCreatedById(creatorId);

        var newId = await _repositoryManager.ServerRepository.CreateAsync(newServer);

        return newId;
    }

    private async Task<Guid> GetCreatorId(string? email)
    {
        // There is a possibility that a user does not have any data in the database.
        // This happens when the integration event handler that registers the user throws an error.
        // We will return an empty Guid just to proceed, later we will handle all servers that does have empty createdById

        if (string.IsNullOrEmpty(email))
            return Guid.Empty;

        var result = await _repositoryManager.UserRepository.GetByEmailAsync(email);
        if (result is null)
            return Guid.Empty;

        return result.Id;
    }
}

