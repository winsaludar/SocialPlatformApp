using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.ServerId).NotEmpty().MustBeExistingServer(repositoryManager);
        RuleFor(x => x.ChannelId).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);

        RuleFor(x => new Tuple<Guid, Guid>(x.ServerId, x.ChannelId)).MustBeExistingChannel(repositoryManager);
    }
}
