using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IUserManager _userManager;

    public CreateServerCommandHandler(IRepositoryManager repositoryManager, IUserManager userManager)
    {
        _repositoryManager = repositoryManager;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        Server newServer = new(request.Name, request.ShortDescription, request.LongDescription, request.CreatorEmail, request.Thumbnail);
        Guid userId = await _userManager.GetUserIdByEmailAsync(request.CreatorEmail);
        newServer.SetCreatedById(userId);

        var newId = await _repositoryManager.ServerRepository.CreateAsync(newServer);

        return newId;
    }
}

