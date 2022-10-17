using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
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

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CreatedById).NotEmpty();

        RuleFor(x => new Tuple<Server, string>(x.TargetServer, x.Name)).MustAsync(BeNotExistingName);
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeNotExistingName(Tuple<Server, string> props, CancellationToken cancellationToken)
    {
        (Server targetServer, string channelName) = props;

        var server = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (server is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (server.Channels.Any(x => x.Name.ToLower() == channelName.ToLower()))
            throw new ChannelNameAlreadyExistException(channelName);

        return true;
    }
}
