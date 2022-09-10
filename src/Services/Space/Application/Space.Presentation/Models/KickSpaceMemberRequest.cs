using System.ComponentModel.DataAnnotations;

namespace Space.Presentation.Models;

public class KickSpaceMemberRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
