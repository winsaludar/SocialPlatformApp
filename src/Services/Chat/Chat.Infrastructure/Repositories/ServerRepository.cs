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
        FilterDefinitionBuilder<ServerDbModel> filterBuilder = Builders<ServerDbModel>.Filter;
        FilterDefinition<ServerDbModel> filter = filterBuilder.Eq(x => x.Guid, server.Id.ToString());

        var existingServer = await _serversCollection.Find(filter).FirstOrDefaultAsync();
        if (existingServer == null)
            return;

        // Update server
        UpdateDefinitionBuilder<ServerDbModel> updateBuilder = Builders<ServerDbModel>.Update;
        UpdateDefinition<ServerDbModel> update = updateBuilder.Set(x => x.Name, server.Name)
            .Set(x => x.ShortDescription, server.ShortDescription)
            .Set(x => x.LongDescription, server.LongDescription)
            .Set(x => x.Thumbnail, server.Thumbnail)
            .Set(x => x.LastModifiedById, server.LastModifiedById.ToString())
            .Set(x => x.DateLastModified, DateTime.UtcNow);

        // Add/Update channels
        List<ChannelDbModel> channels = new();
        foreach (var item in server.Channels)
        {
            ChannelDbModel channel = new()
            {
                Guid = item.Id.ToString(),
                Name = item.Name,
                IsPublic = item.IsPublic,
                DateCreated = item.DateCreated,
                CreatedById = item.CreatedById.ToString(),
                LastModifiedById = item.LastModifiedById?.ToString(),
                DateLastModified = item.DateLastModified,
                Members = item.Members.ToList()
            };
            channels.Add(channel);
        }
        update = update.Set(x => x.Channels, channels);

        // Add/Update members
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
        update = update.Set(x => x.Members, members);

        // Add/Update moderators
        List<ModeratorDbModel> moderators = new();
        foreach (var item in server.Moderators)
        {
            ModeratorDbModel moderator = new()
            {
                UserId = item.UserId,
                DateStarted = item.DateStarted
            };
            moderators.Add(moderator);
        }
        update = update.Set(x => x.Moderators, moderators);

        await _serversCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serversCollection.DeleteOneAsync(x => x.Guid.ToLower() == id.ToString().ToLower());
    }

    private static Server CreateServerFromDbModel(ServerDbModel dbModel, bool includeChannels = true, bool includeMembers = true, bool includeModerators = true)
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

                server.AddChannel(id, x.Name, x.IsPublic, createdById, x.DateCreated, lastModifiedById, x.DateLastModified);

                // Add channel members
                Channel channel = server.Channels.FirstOrDefault(x => x.Id == id)!;
                x.Members.ForEach(y => channel.AddMember(y));
            });
        }

        // Add members
        if (includeMembers)
        {
            dbModel.Members.ForEach(x => server.AddMember(x.UserId, x.Username, x.DateJoined));
        }

        // Add moderators
        if (includeModerators)
        {
            dbModel.Moderators.ForEach(x => server.AddModerator(x.UserId, x.DateStarted));
        }

        return server;
    }
}
