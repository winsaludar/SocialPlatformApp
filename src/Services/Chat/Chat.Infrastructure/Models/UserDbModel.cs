using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class UserDbModel : EntityDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("authId")]
    public string AuthId { get; set; } = default!;

    [BsonElement("username")]
    public string Username { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;
}
