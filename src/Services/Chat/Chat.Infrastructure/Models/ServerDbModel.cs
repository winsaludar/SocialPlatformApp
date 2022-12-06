using Chat.Domain.Aggregates.ServerAggregate;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class ServerDbModel : EntityDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("shortDescription")]
    public string ShortDescription { get; set; } = default!;

    [BsonElement("longDescription")]
    public string LongDescription { get; set; } = default!;

    [BsonElement("creatorEmail")]
    public string CreatorEmail { get; set; } = default!;

    [BsonElement("thumbnail")]
    public string? Thumbnail { get; set; }

    [BsonElement("categories")]
    public List<CategoryDbModel> Categories { get; set; } = new();

    [BsonElement("channels")]
    public List<ChannelDbModel> Channels { get; set; } = new();

    [BsonElement("members")]
    public List<MemberDbModel> Members { get; set; } = new();

    [BsonElement("moderators")]
    public List<ModeratorDbModel> Moderators { get; set; } = new();

    public Server CreateServerFromDbModel(bool includeChannels = true, bool includeMembers = true, bool includeModerators = true)
    {
        Server server = new(Name, ShortDescription, LongDescription, CreatorEmail, Thumbnail);
        server.SetId(System.Guid.Parse(Guid));
        server.SetCreatedById(System.Guid.Parse(CreatedById));
        server.SetDateCreated(DateCreated);
        if (!string.IsNullOrEmpty(LastModifiedById))
            server.SetLastModifiedById(System.Guid.Parse(LastModifiedById));
        if (DateLastModified.HasValue)
            server.SetDateLastModified(DateLastModified.Value);

        Categories.ForEach(x => server.AddCategory(new Category(x.Name, x.Id)));

        // Add channels
        if (includeChannels)
        {
            Channels.ForEach(x =>
            {
                Guid id = System.Guid.Parse(x.Guid);
                Guid createdById = System.Guid.Parse(x.CreatedById);
                Guid? lastModifiedById = !string.IsNullOrEmpty(x.LastModifiedById) ? System.Guid.Parse(x.LastModifiedById) : null;

                server.AddChannel(id, x.Name, x.IsPublic, createdById, x.DateCreated, lastModifiedById, x.DateLastModified);

                // Add channel members
                Channel channel = server.Channels.FirstOrDefault(x => x.Id == id)!;
                x.Members.ForEach(y => channel.AddMember(y));
            });
        }

        // Add members
        if (includeMembers)
        {
            Members.ForEach(x => server.AddMember(x.UserId, x.Username, x.DateJoined));
        }

        // Add moderators
        if (includeModerators)
        {
            Moderators.ForEach(x => server.AddModerator(x.UserId, x.DateStarted));
        }

        return server;
    }
}
