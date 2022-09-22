using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Infrastructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Chat.Infrastructure.Repositories;

public class ServerRepository : IServerRepository
{
    private readonly IMongoCollection<ServerDbModel> _serversCollection;

    public ServerRepository(IOptions<ChatDbSettings> chatDbSettings)
    {
        MongoClient mongoClient = new(chatDbSettings.Value.DefaultConnection);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(chatDbSettings.Value.DatabaseName);
        _serversCollection = mongoDatabase.GetCollection<ServerDbModel>(chatDbSettings.Value.ServersCollectionName);
    }

    public async Task<Server?> GetByNameAsync(string name)
    {
        var result = await _serversCollection.Find(x => x.Name == name).FirstOrDefaultAsync();
        if (result == null)
            return null;

        Server server = new(result.Name, result.ShortDescription, result.LongDescription, result.Thumbnail);
        server.SetId(Guid.Parse(result.Guid));
        return server;
    }

    public async Task<Guid> AddAsync(Server newServer)
    {
        Guid guid = Guid.NewGuid();

        ServerDbModel model = new()
        {
            Guid = guid.ToString(),
            Name = newServer.Name,
            ShortDescription = newServer.ShortDescription,
            LongDescription = newServer.LongDescription,
            Thumbnail = newServer.Thumbnail
        };
        await _serversCollection.InsertOneAsync(model);

        return guid;
    }
}
