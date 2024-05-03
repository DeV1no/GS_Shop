using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Infrastructure.Helpers;

public class UserClaimChecker
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string ClaimName { get; set; }

    public UserClaimChecker(IHttpContextAccessor httpContextAccessor, string claimName)
    {
        _httpContextAccessor = httpContextAccessor;
        ClaimName = claimName;
    }

    public bool Check()
    {
        var userClaims = _httpContextAccessor.HttpContext?.User;

        return userClaims != null && userClaims.Claims.Any(c => string
            .Equals(c.Type.ToLower(), ClaimName.ToLower() , StringComparison.CurrentCultureIgnoreCase));
    }
}