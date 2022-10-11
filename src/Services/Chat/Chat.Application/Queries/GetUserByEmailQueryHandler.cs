using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetUserByEmailQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken) =>
        await _repositoryManager.UserRepository.GetByEmailAsync(request.Email);
}
