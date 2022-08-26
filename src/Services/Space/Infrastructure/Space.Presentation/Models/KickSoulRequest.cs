using System.ComponentModel.DataAnnotations;

namespace Space.Presentation.Models;

public class KickSoulRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
