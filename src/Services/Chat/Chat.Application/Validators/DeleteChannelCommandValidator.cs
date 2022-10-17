using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class DeleteChannelCommandValidator : AbstractValidator<DeleteChannelCommand>
{
    private IRepositoryManager _repositoryManager;

    public DeleteChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.TargetChannelId).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.TargetChannelId)).MustAsync(BeExistingChannel);
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeExistingChannel(Tuple<Server, Guid> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid targetChannelId) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (!server.Channels.Any(x => x.Id == targetChannelId))
            throw new ChannelNotFoundException(targetChannelId.ToString());

        return true;
    }
}
