using System.ComponentModel.DataAnnotations;

namespace Chat.Presentation.Models;

public record CreateServerRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string ShortDescription { get; set; } = default!;

    [Required]
    public string LongDescription { get; set; } = default!;

    public string? Thumbnail { get; set; }
}
