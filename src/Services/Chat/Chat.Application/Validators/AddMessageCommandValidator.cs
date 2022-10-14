using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public AddMessageCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.ServerId).NotEmpty().MustAsync(BeExistingServer);
        RuleFor(x => x.ChannelId).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);

        RuleFor(x => new Tuple<Guid, Guid>(x.ServerId, x.ChannelId)).MustAsync(BeExistingChannel);
    }

    private async Task<bool> BeExistingServer(Guid serverId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(serverId);
        if (result is null)
            throw new ServerNotFoundException(serverId.ToString());

        return true;
    }

    private async Task<bool> BeExistingChannel(Tuple<Guid, Guid> props, CancellationToken cancellationToken)
    {
        (Guid serverId, Guid channelId) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(serverId);
        if (server is null)
            throw new ServerNotFoundException(serverId.ToString());

        if (!server.Channels.Any(x => x.Id == channelId))
            throw new ChannelNotFoundException(channelId.ToString());

        return true;
    }
}
