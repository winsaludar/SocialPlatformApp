using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class RemoveChannelMemberCommandValidator : AbstractValidator<RemoveChannelMemberCommand>
{
    public RemoveChannelMemberCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.ChannelId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().MustBeExistingUser(repositoryManager);

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.RemovedById)).MustBeTheCreatorOrAModerator(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.ChannelId)).MustBeExistingChannel(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid, Guid>(x.TargetServer, x.ChannelId, x.UserId)).MustBeExistingMemberOfTheChannel(repositoryManager); // Note: No need to check if user is currently a member of the server to avoid deadlock
    }
}
