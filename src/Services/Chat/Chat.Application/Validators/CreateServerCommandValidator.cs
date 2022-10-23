using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateServerCommandValidator : AbstractValidator<CreateServerCommand>
{
    public CreateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MustNotBeExistingServerName(repositoryManager);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();
        RuleFor(x => x.CreatorEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CreatedById).NotEmpty();
    }
}
