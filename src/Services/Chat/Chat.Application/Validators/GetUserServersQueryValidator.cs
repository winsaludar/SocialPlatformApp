using Chat.Application.Extensions;
using Chat.Application.Queries;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class GetUserServersQueryValidator : AbstractValidator<GetUserServersQuery>
{
    public GetUserServersQueryValidator(IRepositoryManager repositoryManager) => RuleFor(x => x.UserId).NotNull().MustBeExistingUser(repositoryManager);
}
