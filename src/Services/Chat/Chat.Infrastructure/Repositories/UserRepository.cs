﻿using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Infrastructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDbModel> _usersCollection;
    private readonly IMongoCollection<ServerDbModel> _serversCollection;

    public UserRepository(IOptions<ChatDbSettings> chatDbSettings)
    {
        MongoClient mongoClient = new(chatDbSettings.Value.DefaultConnection);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(chatDbSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<UserDbModel>(chatDbSettings.Value.UsersCollectionName);
        _serversCollection = mongoDatabase.GetCollection<ServerDbModel>(chatDbSettings.Value.ServersCollectionName);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var result = await _usersCollection.Find(x => x.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return CreateUserFromDbModel(result);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var result = await _usersCollection.Find(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return CreateUserFromDbModel(result);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var result = await _usersCollection.Find(x => x.Guid.ToLower() == id.ToString()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return CreateUserFromDbModel(result);
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

    public async Task<IEnumerable<Server>> GetUserServers(Guid id)
    {
        var result = await _serversCollection.Find(x => x.CreatedById.ToLower() == id.ToString().ToLower() ||
            x.Members.Any(y => y.UserId == id)).ToListAsync();

        if (result == null || !result.Any())
            return Enumerable.Empty<Server>();

        List<Server> servers = new();
        result.ForEach(x => servers.Add(x.CreateServerFromDbModel()));
        return servers;
    }

    private static User CreateUserFromDbModel(UserDbModel dbModel)
    {
        User user = new(Guid.Parse(dbModel.AuthId), dbModel.Username, dbModel.Email);
        user.SetId(Guid.Parse(dbModel.Guid));
        user.SetCreatedById(Guid.Parse(dbModel.CreatedById));
        user.SetDateCreated(dbModel.DateCreated);
        if (!string.IsNullOrEmpty(dbModel.LastModifiedById))
            user.SetLastModifiedById(Guid.Parse(dbModel.LastModifiedById));
        if (dbModel.DateLastModified.HasValue)
            user.SetDateLastModified(dbModel.DateLastModified.Value);

        return user;
    }
}
