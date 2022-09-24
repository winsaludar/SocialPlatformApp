using Chat.Application.Queries;
using FluentValidation;

namespace Chat.Application.Validators;

public class GetServersQueryValidator : AbstractValidator<GetServersQuery>
{
    public GetServersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
