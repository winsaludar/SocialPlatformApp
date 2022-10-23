using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class DeleteChannelCommandValidator : AbstractValidator<DeleteChannelCommand>
{
    private IRepositoryManager _repositoryManager;

    public DeleteChannelCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => x.TargetChannelId).NotEmpty();
        RuleFor(x => x.DeletedById).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.TargetChannelId)).MustBeExistingChannel(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.DeletedById)).MustBeTheCreatorOrAModerator(repositoryManager);
    }
}
