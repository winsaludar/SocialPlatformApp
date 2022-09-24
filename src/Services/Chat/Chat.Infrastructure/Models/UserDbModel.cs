using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class UserDbModel : BaseDbModel
{
    [BsonElement("authId")]
    public string AuthId { get; set; } = default!;

    [BsonElement("username")]
    public string Username { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;
}
