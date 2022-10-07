using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class UpdateChannelCommandValidator : AbstractValidator<UpdateChannelCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServerId).NotEmpty().MustAsync(BeExistingServer);
        RuleFor(x => x.TargetChannelId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UpdatedBy).NotEmpty().EmailAddress();

        RuleFor(x => new Tuple<Guid, Guid>(x.TargetServerId, x.TargetChannelId)).MustAsync(BeExistingChannel);
        RuleFor(x => new Tuple<Guid, Guid, string>(x.TargetServerId, x.TargetChannelId, x.Name)).MustAsync(BeNotExistingName);
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

    private async Task<bool> BeNotExistingName(Tuple<Guid, Guid, string> props, CancellationToken cancellationToken)
    {
        (Guid targetServerId, Guid targetChannelId, string channelName) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (server is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        var channel = server.Channels.FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());
        if (channel is not null && channel.Id != targetChannelId)
            throw new ChannelNameAlreadyExistException(channelName);

        return true;
    }
}
