using System.ComponentModel.DataAnnotations;

namespace Space.Presentation.Models;

public class CreateSpaceRequest
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
