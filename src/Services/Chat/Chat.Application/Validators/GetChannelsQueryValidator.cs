using Chat.Application.Extensions;
using Chat.Application.Queries;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class GetChannelsQueryValidator : AbstractValidator<GetChannelsQuery>
{
    public GetChannelsQueryValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.TargetServerId).NotEmpty().MustBeExistingServer(repositoryManager);
    }
}
