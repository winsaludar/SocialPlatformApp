using Microsoft.AspNetCore.Identity;

namespace Authentication.Persistence.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public IEnumerable<RefreshToken> RefreshTokens { get; set; } = default!;
}
