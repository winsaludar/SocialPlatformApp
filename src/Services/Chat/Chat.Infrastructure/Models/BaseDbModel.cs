using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public abstract class BaseDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("rowId")]
    public string? RowId { get; set; }

    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("createdById")]
    public string CreatedById { get; set; }

    [BsonElement("dateCreated")]
    public DateTime DateCreated { get; set; }

    [BsonElement("lastModifiedById")]
    public string? LastModifiedById { get; set; }

    [BsonElement("dateLastModified")]
    public DateTime? DateLastModified { get; set; }
}
