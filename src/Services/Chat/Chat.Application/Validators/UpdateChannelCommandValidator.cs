using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class UpdateChannelCommandValidator : AbstractValidator<UpdateChannelCommand>
{
    public UpdateChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.TargetChannelId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UpdatedById).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.TargetChannelId)).MustBeExistingChannel(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid, string>(x.TargetServer, x.TargetChannelId, x.Name)).MustNotBeExistingChannelName(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UpdatedById)).MustBeTheCreatorOrAModerator(repositoryManager);
    }
}
