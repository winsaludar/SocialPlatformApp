using Chat.Application.Commands;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public CreateUserCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.AuthId).NotEmpty();
        RuleFor(x => x.Username).NotEmpty().MustAsync(BeNotExistingUsername);
        RuleFor(x => x.Email).NotEmpty().MustAsync(BeNotExistingEmail);
    }

    private async Task<bool> BeNotExistingUsername(string username, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.UserRepository.GetByUsernameAsync(username);
        if (result is null)
            return true;

        throw new UsernameAlreadyExistException(username);
    }

    private async Task<bool> BeNotExistingEmail(string email, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.UserRepository.GetByEmailAsync(email);
        if (result is null)
            return true;

        throw new EmailAlreadyExistException(email);
    }
}
