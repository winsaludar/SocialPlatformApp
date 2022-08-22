using System.ComponentModel.DataAnnotations;

namespace Authentication.Presentation.Models;

public class TokenRequest
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public string RefreshToken { get; set; } = default!;
}
