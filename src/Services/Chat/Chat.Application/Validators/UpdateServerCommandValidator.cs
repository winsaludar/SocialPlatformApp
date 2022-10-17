using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;

        RuleFor(x => x.TargetServer).MustAsync(BeExistingServer);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MustAsync(BeNotExistingName);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();
        RuleFor(x => x.UpdatedById).NotEmpty();

        RuleFor(x => new Tuple<Server, Guid>(x.TargetServer, x.UpdatedById)).MustAsync(BeTheSameWithCreator);
    }

    private async Task<bool> BeNotExistingName(string name, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByNameAsync(name);
        if (result is not null && result.Name.ToLower() != name.ToLower())
            throw new ServerNameAlreadyExistException(name);

        return true;
    }

    private async Task<bool> BeExistingServer(Server targetServer, CancellationToken cancellationToken)
    {
        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        return true;
    }

    private async Task<bool> BeTheSameWithCreator(Tuple<Server, Guid> props, CancellationToken cancellationToken)
    {
        (Server targetServer, Guid updatedById) = props;

        var result = await _repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
        if (result is null)
            throw new ServerNotFoundException(targetServer.Id.ToString());

        if (result.CreatedById != updatedById)
            throw new UnauthorizedUserException(updatedById.ToString());

        return true;
    }
}
