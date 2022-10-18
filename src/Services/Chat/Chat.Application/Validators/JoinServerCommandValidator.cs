using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class JoinServerCommandValidator : AbstractValidator<JoinServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public JoinServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid, string>(x.TargetServer, x.UserId, x.Username)).MustAsync(BeNotExistingMember);
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeNotExistingMember(Tuple<Server, Guid, string> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid userId, string username) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (server.Members.Any(x => x.UserId == userId))
            throw new UserIsAlreadyAMemberException(username);

        return true;
    }
}
