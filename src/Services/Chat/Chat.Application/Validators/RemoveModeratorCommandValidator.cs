using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class RemoveModeratorCommandValidator : AbstractValidator<RemoveModeratorCommand>
{
    public RemoveModeratorCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.UserId).NotEmpty(); // Note: No need to check if user is valid, we just remove it from the list to avoid deadlock

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.RemoveById)).MustBeTheCreator(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustBeExistingModerator(repositoryManager); // Note: No need to check if user is still a member, we just remove it from the list to avoid deadlock
    }
}
