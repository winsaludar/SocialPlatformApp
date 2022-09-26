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
        RuleFor(x => x.DeleterEmail).NotEmpty().EmailAddress();
        RuleFor(x => new Tuple<Guid, string>(x.TargetServerId, x.DeleterEmail)).MustAsync(BeTheSameEmailWithCreator);
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }

    private async Task<bool> BeTheSameEmailWithCreator(Tuple<Guid, string> props, CancellationToken cancellationToken)
    {
        (Guid targetServerId, string deleterEmail) = props;

        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        if (result.CreatorEmail.ToLower() != deleterEmail.ToLower())
            throw new UnauthorizedServerDeleterException(deleterEmail);

        return true;
    }
}
