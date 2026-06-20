using GS_Shop_UserManagement.Infrastructure.Models;
using System.Security.Claims;
using GS_Shop_UserManagement.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GS_Shop_UserManagement.Infrastructure.Auth
{
    public class RedisAuthorizationRequirement : IAuthorizationRequirement;

    public class RedisAuthorizationHandler : AuthorizationHandler<RedisAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisCacheService _redisCacheService;

        public RedisAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IRedisCacheService redisCacheService)
        {
            _httpContextAccessor = httpContextAccessor;
            _redisCacheService = redisCacheService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RedisAuthorizationRequirement requirement)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var redisKey = user.FindFirstValue("redisKey");
            if (string.IsNullOrEmpty(redisKey))
            {
                redisKey = user.Claims.FirstOrDefault(x => x.Type == "redisKey")?.Value;
            }

            if (!string.IsNullOrEmpty(redisKey))
            {
                var claimsJson = await _redisCacheService.GetAsync(redisKey);
                if (!string.IsNullOrEmpty(claimsJson))
                {
                    var payload = JsonConvert.DeserializeObject<RedisClaims>(claimsJson);

                    if (payload != null)
                    {
                        if (user.Identity is ClaimsIdentity claimsIdentity)
                        {
                            if (payload.Permissions != null)
                            {
                                foreach (var permission in payload.Permissions)
                                {
                                    if (!string.IsNullOrEmpty(permission.Type) && !string.IsNullOrEmpty(permission.Value))
                                    {
                                        if (!claimsIdentity.HasClaim(permission.Type, permission.Value))
                                        {
                                            claimsIdentity.AddClaim(new Claim(permission.Type, permission.Value));
                                        }
                                    }
                                }
                            }

                            if (payload.Limitations != null)
                            {
                                foreach (var limitation in payload.Limitations)
                                {
                                    if (!string.IsNullOrEmpty(limitation.Type) && !string.IsNullOrEmpty(limitation.Value))
                                    {
                                        if (!claimsIdentity.HasClaim(limitation.Type, limitation.Value))
                                        {
                                            claimsIdentity.AddClaim(new Claim(limitation.Type, limitation.Value));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }
}