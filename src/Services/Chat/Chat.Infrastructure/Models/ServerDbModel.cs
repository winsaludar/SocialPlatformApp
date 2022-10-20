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

    [BsonElement("channels")]
    public List<ChannelDbModel> Channels { get; set; } = new();

    [BsonElement("members")]
    public List<MemberDbModel> Members { get; set; } = new();

    [BsonElement("moderators")]
    public List<ModeratorDbModel> Moderators { get; set; } = new();
}
