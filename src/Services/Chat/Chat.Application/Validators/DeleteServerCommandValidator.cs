using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class DeleteServerCommandValidator : AbstractValidator<DeleteServerCommand>
{
    public DeleteServerCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServerId).NotEmpty().MustBeExistingServer(repositoryManager);
        RuleFor(x => x.DeletedById).NotEmpty();
        RuleFor(x => new Tuple<Guid, Guid>(x.TargetServerId, x.DeletedById)).MustBeTheCreator(repositoryManager);
    }
}
