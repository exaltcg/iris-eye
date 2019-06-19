using System;
using System.Security.Claims;

namespace IrisEye.Web.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        
        public static bool IsInRole(this ClaimsPrincipal principal, SystemRoles role)
        {
            return principal.IsInRole(role.ToString());
        }
    }

}