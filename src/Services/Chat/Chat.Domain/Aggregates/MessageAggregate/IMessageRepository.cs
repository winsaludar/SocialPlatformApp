using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.MessageAggregate;

public interface IMessageRepository : IRepository<Message>
{
    Task<Guid> CreateAsync(Message newMessage);
}
