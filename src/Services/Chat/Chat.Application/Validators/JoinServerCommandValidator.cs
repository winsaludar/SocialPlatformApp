using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class JoinServerCommandValidator : AbstractValidator<JoinServerCommand>
{
    public JoinServerCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.UserId).NotEmpty().MustBeExistingUser(repositoryManager);
        RuleFor(x => x.Username).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustNotBeExistingMember(repositoryManager);
    }
}
