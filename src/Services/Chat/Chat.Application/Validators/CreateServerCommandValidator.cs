using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateServerCommandValidator : AbstractValidator<CreateServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(BeNotExistingName);

        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();
        RuleFor(x => x.CreatorEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CreatedById).NotEmpty();
    }

    private async Task<bool> BeNotExistingName(string name, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByNameAsync(name);
        if (result is null)
            return true;

        throw new ServerNameAlreadyExistException(name);
    }
}
