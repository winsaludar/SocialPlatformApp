using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class MessageDbModel : EntityDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("serverId")]
    [BsonRepresentation(BsonType.String)]
    public string ServerId { get; set; } = default!;

    [BsonElement("channelId")]
    [BsonRepresentation(BsonType.String)]
    public string ChannelId { get; set; } = default!;

    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.String)]
    public string SenderId { get; set; } = default!;

    [BsonElement("username")]
    public string Username { get; set; } = default!;

    [BsonElement("content")]
    public string Content { get; set; } = default!;
}
