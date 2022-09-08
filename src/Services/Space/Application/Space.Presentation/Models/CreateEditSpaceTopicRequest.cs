using System.ComponentModel.DataAnnotations;

namespace Space.Presentation.Models;

public class CreateEditSpaceTopicRequest
{
    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;
}
