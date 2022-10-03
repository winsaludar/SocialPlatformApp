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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var result = await _usersCollection.Find(x => x.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        User user = new(Guid.Parse(result.AuthId), result.Username, result.Email);
        user.SetId(Guid.Parse(result.Guid));
        user.SetCreatedById(Guid.Parse(result.CreatedById));
        user.SetDateCreated(result.DateCreated);
        if (!string.IsNullOrEmpty(result.LastModifiedById))
            user.SetLastModifiedById(Guid.Parse(result.LastModifiedById));
        if (result.DateLastModified.HasValue)
            user.SetDateLastModified(result.DateLastModified.Value);

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var result = await _usersCollection.Find(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        User user = new(Guid.Parse(result.AuthId), result.Username, result.Email);
        user.SetId(Guid.Parse(result.Guid));
        user.SetCreatedById(Guid.Parse(result.CreatedById));
        user.SetDateCreated(result.DateCreated);
        if (!string.IsNullOrEmpty(result.LastModifiedById))
            user.SetLastModifiedById(Guid.Parse(result.LastModifiedById));
        if (result.DateLastModified.HasValue)
            user.SetDateLastModified(result.DateLastModified.Value);

        return user;
    }

    public async Task<Guid> AddAsync(User newUser)
    {
        Guid newId = Guid.NewGuid();

        UserDbModel model = new()
        {
            Guid = newId.ToString(),
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
