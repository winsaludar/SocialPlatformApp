using Chat.Domain.Aggregates.UserAggregate;
using Chat.Infrastructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDbModel> _usersCollection;

    public UserRepository(IOptions<ChatDbSettings> chatDbSettings)
    {
        MongoClient mongoClient = new(chatDbSettings.Value.DefaultConnection);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(chatDbSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<UserDbModel>(chatDbSettings.Value.UsersCollectionName);
    }

    public async Task<Guid> AddAsync(User newUser)
    {
        Guid newId = Guid.NewGuid();

        UserDbModel model = new()
        {
            Id = newId.ToString(),
            AuthId = newUser.AuthId.ToString(),
            Username = newUser.Username,
            Email = newUser.Email,
            CreatedById = newUser.CreatedById.ToString(),
            DateCreated = DateTime.UtcNow
        };
        await _usersCollection.InsertOneAsync(model);

        return newId;
    }
}
