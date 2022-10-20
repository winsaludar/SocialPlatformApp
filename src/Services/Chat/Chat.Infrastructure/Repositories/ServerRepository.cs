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
        result.ForEach(x => servers.Add(CreateServerFromDbModel(x, includeChannels: false, includeMembers: false)));

        return servers;
    }

    public async Task<Server?> GetByNameAsync(string name)
    {
        var result = await _serversCollection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return CreateServerFromDbModel(result);
    }

    public async Task<Server?> GetByIdAsync(Guid id)
    {
        var result = await _serversCollection.Find(x => x.Guid.ToLower() == id.ToString().ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return CreateServerFromDbModel(result);
    }

    public async Task<Guid> CreateAsync(Server newServer)
    {
        Guid newId = Guid.NewGuid();

        ServerDbModel model = new()
        {
            Guid = newId.ToString(),
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
        var result = await _serversCollection.Find(x => x.Guid.ToLower() == server.Id.ToString().ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return;

        // TODO: Refactor updating; Do not fetch channels & members and re-insert every update
        // Just insert if channel/member has value

        // Update server
        ServerDbModel model = new()
        {
            Id = result.Id,
            Guid = result.Guid.ToString(),
            Name = server.Name,
            ShortDescription = server.ShortDescription,
            LongDescription = server.LongDescription,
            CreatorEmail = result.CreatorEmail,
            Thumbnail = server.Thumbnail,
            CreatedById = result.CreatedById.ToString(),
            DateCreated = result.DateCreated,
            LastModifiedById = server.LastModifiedById.ToString(),
            DateLastModified = DateTime.UtcNow,
        };

        // Update channels
        List<ChannelDbModel> channels = new();
        foreach (var item in server.Channels)
        {
            ChannelDbModel channel = new()
            {
                Guid = item.Id.ToString(),
                Name = item.Name,
                DateCreated = item.DateCreated,
                CreatedById = item.CreatedById.ToString()
            };

            if (item.DateLastModified.HasValue)
                channel.DateLastModified = item.DateLastModified.Value;

            if (item.LastModifiedById.HasValue)
                channel.LastModifiedById = item.LastModifiedById.ToString();

            channels.Add(channel);
        }
        model.Channels = channels;

        // Update members
        List<MemberDbModel> members = new();
        foreach (var item in server.Members)
        {
            MemberDbModel member = new()
            {
                UserId = item.UserId,
                Username = item.Username,
                DateJoined = item.DateJoined
            };
            members.Add(member);
        }
        model.Members = members;

        await _serversCollection.ReplaceOneAsync(x => x.Guid.ToLower() == server.Id.ToString().ToLower(), model);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serversCollection.DeleteOneAsync(x => x.Guid.ToLower() == id.ToString().ToLower());
    }

    private static Server CreateServerFromDbModel(ServerDbModel dbModel, bool includeChannels = true, bool includeMembers = true)
    {
        Server server = new(dbModel.Name, dbModel.ShortDescription, dbModel.LongDescription, dbModel.CreatorEmail, dbModel.Thumbnail);
        server.SetId(Guid.Parse(dbModel.Guid));
        server.SetCreatedById(Guid.Parse(dbModel.CreatedById));
        server.SetDateCreated(dbModel.DateCreated);
        if (!string.IsNullOrEmpty(dbModel.LastModifiedById))
            server.SetLastModifiedById(Guid.Parse(dbModel.LastModifiedById));
        if (dbModel.DateLastModified.HasValue)
            server.SetDateLastModified(dbModel.DateLastModified.Value);

        // Add channels
        if (includeChannels)
        {
            dbModel.Channels.ForEach(x =>
            {
                Guid id = Guid.Parse(x.Guid);
                Guid createdById = Guid.Parse(x.CreatedById);
                Guid? lastModifiedById = !string.IsNullOrEmpty(x.LastModifiedById) ? Guid.Parse(x.LastModifiedById) : null;

                server.AddChannel(id, x.Name, createdById, x.DateCreated, lastModifiedById, x.DateLastModified);
            });
        }

        // Add members
        if (includeMembers)
        {
            dbModel.Members.ForEach(x => server.AddMember(x.UserId, x.Username, x.DateJoined));
        }

        return server;
    }
}
