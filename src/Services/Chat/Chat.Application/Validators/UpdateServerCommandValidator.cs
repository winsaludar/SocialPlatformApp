using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MustNotBeExistingServerName(repositoryManager);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();
        RuleFor(x => x.UpdatedById).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UpdatedById)).MustBeTheCreator(repositoryManager);
    }
}
