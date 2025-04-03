using System.Security.Claims;

namespace ShandaApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        public static bool IsKid(this ClaimsPrincipal user)
        {
            return user.IsInRole("Kid");
        }

        public static bool IsUser(this ClaimsPrincipal user)
        {
            return user.IsInRole("User");
        }
    }
}
