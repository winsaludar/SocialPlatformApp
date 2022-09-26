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

    public async Task<IEnumerable<Server>> GetAllAsync(int? skip = null, int? limit = null, string? nameFilter = null)
    {

        IFindFluent<ServerDbModel, ServerDbModel> query;
        if (!string.IsNullOrEmpty(nameFilter))
            query = _serversCollection.Find(x => x.Name.ToLower().Contains(nameFilter.ToLower()));
        else
            query = _serversCollection.Find(_ => true);

        if (limit.HasValue)
            query = query.Limit(limit.Value);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        var result = await query.ToListAsync();
        if (result == null || !result.Any())
            return Enumerable.Empty<Server>();

        List<Server> servers = new();
        result.ForEach(x =>
        {
            Server server = new(x.Name, x.ShortDescription, x.LongDescription, x.CreatorEmail, x.Thumbnail);
            server.SetId(Guid.Parse(x.Id));
            server.SetCreatedById(Guid.Parse(x.CreatedById));
            server.SetDateCreated(x.DateCreated);
            if (!string.IsNullOrEmpty(x.LastModifiedById))
                server.SetLastModifiedById(Guid.Parse(x.LastModifiedById));
            if (x.DateLastModified.HasValue)
                server.SetDateLastModified(x.DateLastModified.Value);

            servers.Add(server);
        });

        return servers;
    }

    public async Task<Server?> GetByNameAsync(string name)
    {
        var result = await _serversCollection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        Server server = new(result.Name, result.ShortDescription, result.LongDescription, result.CreatorEmail, result.Thumbnail);
        server.SetId(Guid.Parse(result.Id));
        server.SetCreatedById(Guid.Parse(result.CreatedById));
        server.SetDateCreated(result.DateCreated);
        if (!string.IsNullOrEmpty(result.LastModifiedById))
            server.SetLastModifiedById(Guid.Parse(result.LastModifiedById));
        if (result.DateLastModified.HasValue)
            server.SetDateLastModified(result.DateLastModified.Value);

        return server;
    }

    public async Task<Server?> GetByIdAsync(Guid id)
    {
        var result = await _serversCollection.Find(x => x.Id.ToLower() == id.ToString().ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        Server server = new(result.Name, result.ShortDescription, result.LongDescription, result.CreatorEmail, result.Thumbnail);
        server.SetId(Guid.Parse(result.Id));
        server.SetCreatedById(Guid.Parse(result.CreatedById));
        server.SetDateCreated(result.DateCreated);
        if (!string.IsNullOrEmpty(result.LastModifiedById))
            server.SetLastModifiedById(Guid.Parse(result.LastModifiedById));
        if (result.DateLastModified.HasValue)
            server.SetDateLastModified(result.DateLastModified.Value);

        return server;
    }

    public async Task<Guid> CreateAsync(Server newServer)
    {
        Guid newId = Guid.NewGuid();

        ServerDbModel model = new()
        {
            Id = newId.ToString(),
            Name = newServer.Name,
            ShortDescription = newServer.ShortDescription,
            LongDescription = newServer.LongDescription,
            CreatorEmail = newServer.CreatorEmail,
            Thumbnail = newServer.Thumbnail,
            CreatedById = newServer.CreatedById.ToString(),
            DateCreated = DateTime.UtcNow
        };
        await _serversCollection.InsertOneAsync(model);

        return newId;
    }

    public async Task UpdateAsync(Server server)
    {
        var filter = Builders<ServerDbModel>.Filter.Where(x => x.Id.ToLower() == server.Id.ToString().ToLower());
        var data = Builders<ServerDbModel>.Update
            .Set(x => x.Name, server.Name)
            .Set(x => x.ShortDescription, server.ShortDescription)
            .Set(x => x.LongDescription, server.LongDescription)
            .Set(x => x.Thumbnail, server.Thumbnail)
            .Set(x => x.LastModifiedById, server.LastModifiedById.ToString())
            .Set(x => x.DateLastModified, DateTime.UtcNow);

        await _serversCollection.UpdateOneAsync(filter, data);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serversCollection.DeleteOneAsync(x => x.Id.ToLower() == id.ToString().ToLower());
    }
}
