using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class CategoryDbModel
{
    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = default!;
}
