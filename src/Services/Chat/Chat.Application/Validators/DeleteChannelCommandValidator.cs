using Chat.Application.Commands;
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

        RuleFor(x => x.TargetServerId).NotEmpty().MustAsync(BeExistingServer);
        RuleFor(x => x.TargetChannelId).NotEmpty();

        RuleFor(x => new Tuple<Guid, Guid>(x.TargetServerId, x.TargetChannelId)).MustAsync(BeExistingChannel);
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }

    private async Task<bool> BeExistingChannel(Tuple<Guid, Guid> props, CancellationToken cancellationToken)
    {
        (Guid targetServerId, Guid targetChannelId) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (server is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        if (!server.Channels.Any(x => x.Id == targetChannelId))
            throw new ChannelNotFoundException(targetChannelId.ToString());

        return true;
    }
}
