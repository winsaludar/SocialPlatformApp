using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class DeleteServerCommandValidator : AbstractValidator<DeleteServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public DeleteServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServerId).NotEmpty().MustAsync(BeExistingServer);
        RuleFor(x => x.DeletedById).NotEmpty();
        RuleFor(x => new Tuple<Guid, Guid>(x.TargetServerId, x.DeletedById)).MustAsync(BeTheSameWithCreator);
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }

    private async Task<bool> BeTheSameWithCreator(Tuple<Guid, Guid> props, CancellationToken cancellationToken)
    {
        (Guid targetServerId, Guid deletedById) = props;

        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        if (result.CreatedById != deletedById)
            throw new UnauthorizedServerDeleterException(deletedById.ToString());

        return true;
    }
}
