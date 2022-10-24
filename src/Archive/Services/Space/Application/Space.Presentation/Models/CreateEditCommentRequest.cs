using System.ComponentModel.DataAnnotations;

namespace Space.Presentation.Models;

public class CreateEditCommentRequest
{
    [Required]
    public string Content { get; set; } = default!;
}
