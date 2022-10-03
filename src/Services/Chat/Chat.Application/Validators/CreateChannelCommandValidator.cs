using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServerId)
            .NotEmpty()
            .MustAsync(BeExistingServer);

        RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(50);

        RuleFor(x => new Tuple<Guid, string>(x.TargetServerId, x.Name))
            .MustAsync(BeNotExistingName);
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }

    private async Task<bool> BeNotExistingName(Tuple<Guid, string> props, CancellationToken cancellationToken)
    {
        (Guid targetServerId, string channelName) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (server is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        if (server.Channels.Any(x => x.Name.ToLower() == channelName.ToLower()))
            throw new ChannelNameAlreadyExistException(channelName);

        return true;
    }
}
