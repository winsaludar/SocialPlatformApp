using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class ChannelDbModel : EntityDbModel
{
    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("isPublic")]
    public bool IsPublic { get; set; }

    [BsonElement("members")]
    [BsonRepresentation(BsonType.String)]
    public List<Guid> Members { get; set; } = new();
}
