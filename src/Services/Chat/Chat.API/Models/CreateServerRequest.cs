using System.ComponentModel.DataAnnotations;

namespace Chat.API.Models;

public record CreateServerRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string ShortDescription { get; set; } = default!;

    [Required]
    public string LongDescription { get; set; } = default!;

    public string? Thumbnail { get; set; }
}
