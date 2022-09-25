using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServerId)
            .NotEmpty()
            .MustAsync(BeExistingServer);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(BeNotExistingName);

        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();

        // TODO: Make sure editor is the same as the creator
    }

    private async Task<bool> BeNotExistingName(string name, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByNameAsync(name);
        if (result is not null && result.Name.ToLower() != name.ToLower())
            throw new ServerNameAlreadyExistException(name);

        return true;
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }
}
