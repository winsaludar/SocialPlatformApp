using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CreatedById).NotEmpty();

        RuleFor(x => new Tuple<Server, string>(x.TargetServer, x.Name)).MustNotBeExistingChannelName(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.CreatedById)).MustBeTheCreatorOrAModerator(repositoryManager);
    }
}
