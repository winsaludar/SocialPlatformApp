using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand, bool>
{
    private readonly IRepositoryManager _repositoryManager;

    public UpdateChannelCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<bool> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(request.TargetServerId);
        if (server is null)
            throw new ServerNotFoundException(request.TargetServerId.ToString());

        Guid updaterId = await GetUpdaterId(request.UpdatedBy);
        server.UpdateChannel(request.TargetChannelId, request.Name, DateTime.UtcNow, updaterId);

        await _repositoryManager.ServerRepository.UpdateAsync(server);

        return true;
    }

    private async Task<Guid> GetUpdaterId(string? email)
    {
        // There is a possibility that a user does not have any data in the database.
        // This happens when the integration event handler that registers the user throws an error.
        // We will return an empty Guid just to proceed, later we will handle all servers that does have empty createdById

        if (string.IsNullOrEmpty(email))
            return Guid.Empty;

        var result = await _repositoryManager.UserRepository.GetByEmailAsync(email);
        if (result is null)
            return Guid.Empty;

        return result.Id;
    }
}
