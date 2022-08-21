using System.ComponentModel.DataAnnotations;

namespace Authentication.Presentation.Models;

public class RegisterApplicationUser
{
    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]

    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
