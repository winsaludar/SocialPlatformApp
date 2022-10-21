using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class RemoveModeratorCommandValidator : AbstractValidator<RemoveModeratorCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public RemoveModeratorCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.UserId).NotEmpty(); // Note: No need to check if user is valid, we just remove it from the list to avoid deadlock

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.RemoveById)).MustAsync(BeTheCreator);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustAsync(BeExistingModerator); // Note: No need to check if user is still a member, we just remove it from the list to avoid deadlock
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeTheCreator(Tuple<Server, Guid> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid addedById) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (server.CreatedById != addedById)
            throw new UnauthorizedUserException(addedById.ToString());

        return true;
    }

    private async Task<bool> BeExistingModerator(Tuple<Server, Guid> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid userId) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (!server.Moderators.Any(x => x.UserId == userId))
            throw new UserIsNotAModeratorException(userId.ToString());

        return true;
    }
}
