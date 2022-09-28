using System.Security.Claims;

namespace Chat.API.Extensions;

public static class UserContextExtensions
{
    public static bool IsValid(this ClaimsPrincipal? User)
    {
        if (User == null || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return false;

        return true;
    }
}
