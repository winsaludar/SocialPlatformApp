using Chat.Application.DTOs;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Queries;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetUserByEmailQueryHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _repositoryManager.UserRepository.GetByEmailAsync(request.Email);
        if (user is null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}
