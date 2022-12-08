using Chat.Application.Extensions;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class GetUserServerChannelsQueryValidator : AbstractValidator<GetUserServerChannelsQuery>
{
    public GetUserServerChannelsQueryValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.UserId).NotEmpty().MustBeExistingUser(repositoryManager);
        RuleFor(x => x.TargetServer).MustBeExistingServer(repositoryManager);
        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UserId)).MustBeExistingMember(repositoryManager);
    }
}
