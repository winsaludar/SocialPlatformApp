using System.ComponentModel.DataAnnotations;

namespace Authentication.API.Models;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}

