﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public record ServerDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("guid")]
    public string Guid { get; set; } = default!;

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("shortDescription")]
    public string ShortDescription { get; set; } = default!;

    [BsonElement("longDescription")]
    public string LongDescription { get; set; } = default!;

    [BsonElement("thumbnail")]
    public string? Thumbnail { get; set; }
}
