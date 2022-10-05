using Chat.Application.Queries;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class GetChannelsQueryValidator : AbstractValidator<GetChannelsQuery>
{
    private readonly IRepositoryManager _repositoryManager;

    public GetChannelsQueryValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServerId)
            .NotEmpty()
            .MustAsync(BeExistingServer);
    }

    private async Task<bool> BeExistingServer(Guid targetServerId, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
        if (result is null)
            throw new ServerNotFoundException(targetServerId.ToString());

        return true;
    }
}
