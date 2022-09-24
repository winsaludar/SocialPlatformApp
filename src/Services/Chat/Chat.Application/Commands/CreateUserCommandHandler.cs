using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateUserCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User newUser = new(request.AuthId, request.Username, request.Email);
        newUser.SetCreatedById(request.CreatedById);

        var newId = await _repositoryManager.UserRepository.AddAsync(newUser);

        return newId;
    }
}
