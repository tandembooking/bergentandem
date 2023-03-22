using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TandemBooking.Services
{
    public static class ClaimsPrincipalExtensions
    {
        public static string AdminClaim = "IsAdmin";
        public static string PilotClaim = "IsPilot";

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.HasClaim(claim => claim.Type == AdminClaim);
        }

        public static bool IsPilot(this ClaimsPrincipal principal)
        {
            return principal.HasClaim(claim => claim.Type == PilotClaim);
        }
    }
}
