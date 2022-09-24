using Chat.Application.Commands;
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
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}
