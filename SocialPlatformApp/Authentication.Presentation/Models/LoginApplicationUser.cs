using System.ComponentModel.DataAnnotations;

namespace Authentication.Presentation.Models;

public class LoginApplicationUser
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
