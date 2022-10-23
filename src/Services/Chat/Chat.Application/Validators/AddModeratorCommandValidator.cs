using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class AddModeratorCommandValidator : AbstractValidator<AddModeratorCommand>
{
    public AddModeratorCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.UserId).NotEmpty().MustBeExistingUser(repositoryManager);

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.AddedById)).MustBeTheCreatorOrAModerator(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustBeExistingMember(repositoryManager)
            .MustNotBeExistingModerator(repositoryManager);
    }
}
