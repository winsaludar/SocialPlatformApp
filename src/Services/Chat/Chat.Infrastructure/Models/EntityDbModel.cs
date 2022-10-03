using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public abstract class EntityDbModel
{
    [BsonElement("guid")]
    [BsonRepresentation(BsonType.String)]
    public string Guid { get; set; } = default!;

    [BsonElement("createdById")]
    public string CreatedById { get; set; } = default!;

    [BsonElement("dateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("lastModifiedById")]
    public string? LastModifiedById { get; set; }

    [BsonElement("dateLastModified")]
    public DateTime? DateLastModified { get; set; }
}
