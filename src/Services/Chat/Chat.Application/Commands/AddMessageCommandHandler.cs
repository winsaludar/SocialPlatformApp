using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.SeedWork;
using MediatR;

namespace Chat.Application.Commands;

public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, Guid>
{
    private readonly IRepositoryManager _repositoryManager;

    public AddMessageCommandHandler(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        Message msg = new(request.ServerId, request.ChannelId, request.SenderId, request.Username, request.Content);
        msg.SetCreatedById(request.SenderId);

        Guid newId = await _repositoryManager.MessageRepository.CreateAsync(msg);

        return newId;
    }
}
