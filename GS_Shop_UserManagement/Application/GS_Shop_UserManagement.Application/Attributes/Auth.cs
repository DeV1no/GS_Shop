using System.Security.Claims;
using GS_Shop_UserManagement.Application.DTOs.RedisClaims;
using GS_Shop_UserManagement.Infrastructure.Helpers;
using GS_Shop_UserManagement.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GS_Shop_UserManagement.Application.Attributes;

public class Auth : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _requiredClaim;

    public Auth(string requiredClaim)
    {
        AuthenticationSchemes = "Bearer";
        _requiredClaim = requiredClaim;
    }

    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var redisKey = user.Claims.FirstOrDefault(x => x.Type == "redisKey")?.Value;
        if (string.IsNullOrEmpty(redisKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Retrieve IRedisCacheService from ServiceLocator
        var redisCacheService = ServiceLocator.ServiceProvider.GetRequiredService<IRedisCacheService>();
        var redisData = await redisCacheService.GetAsync(redisKey);

        if (redisData == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Deserialize the JSON data into RedisClaims
        var redisClaims = JsonConvert.DeserializeObject<RedisClaims>(redisData);

        if (redisClaims == null || redisClaims.Permissions == null || 
            !redisClaims.Permissions.Any(c => c.Type == _requiredClaim && c.Value == "true"))
        {
            context.Result = new ForbidResult();
        }
    }
}