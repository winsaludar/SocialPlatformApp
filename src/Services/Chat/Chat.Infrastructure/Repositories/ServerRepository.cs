﻿using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
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

    public async Task<IEnumerable<Server>> GetAllAsync(int? skip = null, int? limit = null, string? nameFilter = null, string? categoryFilter = null)
    {

        IFindFluent<ServerDbModel, ServerDbModel> query;


        if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter.ToLower() != "others")
        {
            query = _serversCollection.Find(x =>
                x.Name.ToLower().Contains(nameFilter ?? "") &&
                x.Categories.Any(y => y.Name.ToLower() == categoryFilter.ToLower()));
        }
        else if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter.ToLower() == "others")
        {
            List<string> validCategories = Enumeration.GetAll<Category>().Select(x => x.Name).ToList();

            query = _serversCollection.Find(x =>
                x.Name.ToLower().Contains(nameFilter ?? "") &&
                !x.Categories.Any(y => validCategories.Contains(y.Name)));
        }
        else
        {
            query = _serversCollection.Find(x => x.Name.ToLower().Contains(nameFilter ?? ""));
        }

        if (limit.HasValue)
            query = query.Limit(limit.Value);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        var result = await query.ToListAsync();
        if (result == null || !result.Any())
            return Enumerable.Empty<Server>();

        List<Server> servers = new();
        result.ForEach(x => servers.Add(x.CreateServerFromDbModel(includeChannels: false, includeMembers: false)));

        return servers;
    }

    public async Task<Server?> GetByNameAsync(string name)
    {
        var result = await _serversCollection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return result.CreateServerFromDbModel();
    }

    public async Task<Server?> GetByIdAsync(Guid id)
    {
        var result = await _serversCollection.Find(x => x.Guid.ToLower() == id.ToString().ToLower()).FirstOrDefaultAsync();
        if (result == null)
            return null;

        return result.CreateServerFromDbModel();
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
            DateCreated = DateTime.UtcNow,
        };

        // Add categories
        foreach (var item in newServer.Categories)
            model.Categories.Add(new CategoryDbModel { Id = item.Id, Name = item.Name });

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
}
