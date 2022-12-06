using Chat.Domain.Aggregates.ServerAggregate;

namespace Chat.API.Models;

public record CreateUpdateServerModel
{
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public List<Category> Categories { get; set; } = default!;
    public string? Thumbnail { get; set; }
}
