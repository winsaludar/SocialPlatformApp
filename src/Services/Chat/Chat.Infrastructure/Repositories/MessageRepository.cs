using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Infrastructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<MessageDbModel> _messagesCollection;

    public MessageRepository(IOptions<ChatDbSettings> chatDbSettings)
    {
        MongoClient mongoClient = new(chatDbSettings.Value.DefaultConnection);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(chatDbSettings.Value.DatabaseName);
        _messagesCollection = mongoDatabase.GetCollection<MessageDbModel>(chatDbSettings.Value.MessagesCollectionName);
    }

    public async Task<Guid> CreateAsync(Message newMessage)
    {
        Guid newId = Guid.NewGuid();

        MessageDbModel model = new()
        {
            Guid = newId.ToString(),
            ServerId = newMessage.ServerId.ToString(),
            ChannelId = newMessage.ChannelId.ToString(),
            SenderId = newMessage.SenderId.ToString(),
            Username = newMessage.Username,
            Content = newMessage.Content,
            CreatedById = newMessage.CreatedById.ToString(),
            DateCreated = DateTime.UtcNow
        };

        await _messagesCollection.InsertOneAsync(model);

        return newId;
    }
}
