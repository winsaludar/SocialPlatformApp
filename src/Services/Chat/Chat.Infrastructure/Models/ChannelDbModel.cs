using MongoDB.Bson.Serialization.Attributes;

namespace Chat.Infrastructure.Models;

public class ChannelDbModel : EntityDbModel
{
    public string Name { get; set; } = default!;

    [BsonElement("messages")]
    public List<MessageDbModel> Messages { get; set; } = new();
}
