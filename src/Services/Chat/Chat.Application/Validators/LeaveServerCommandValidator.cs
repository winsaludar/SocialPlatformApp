using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class LeaveServerCommandValidator : AbstractValidator<LeaveServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public LeaveServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.UserId).NotEmpty().MustAsync(BeExistingUser);

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustAsync(BeExistingMember);
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeExistingUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.UserRepository.GetByIdAsync(userId);
        if (result is null)
            throw new UserNotFoundException(userId.ToString());

        return true;
    }

    private async Task<bool> BeExistingMember(Tuple<Server, Guid> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid userId) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (!server.Members.Any(x => x.UserId == userId))
            throw new UserIsNotAMemberException(userId.ToString());

        return true;
    }
}
