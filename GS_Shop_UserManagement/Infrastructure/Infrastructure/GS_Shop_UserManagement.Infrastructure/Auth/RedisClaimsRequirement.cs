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
            var redisKey = _httpContextAccessor.HttpContext.User.FindFirstValue("redisKey");

            if (!string.IsNullOrEmpty(redisKey))
            {
                var claimsJson = await _redisCacheService.GetAsync(redisKey);
                if (!string.IsNullOrEmpty(claimsJson))
                {
                    var payload = JsonConvert.DeserializeObject<RedisClaimsPayload>(claimsJson);

                    var combinedClaimsIdentity = new ClaimsIdentity();

                    foreach (var permission in payload.Permissions)
                    {
                        
                        Claim newClaim = new Claim(
                            permission.Type, permission.Value,
                            "https://www.w3.org/2001/XMLSchema#string", 
                            issuer:"GS_Shop",
                            originalIssuer:"GS_Shop"
                            );

                        // Get the current user's ClaimsIdentity
                        ClaimsIdentity claimsIdentity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
                        // Add the new claim to the ClaimsIdentity
                        claimsIdentity.AddClaim(newClaim);
                    }

                    foreach (var limitation in payload.Limitations)
                    {
                        combinedClaimsIdentity.AddClaim(new Claim(limitation.Type, limitation.Value));
                    }

                    // Replace the user's identity with the one containing claims from Redis
                 
                    // _httpContextAccessor.HttpContext.User.AddIdentity(combinedClaimsIdentity);
                    
                    // Get the required permissions for the policy
                    var requiredPermissions = context.Requirements
                        .OfType<IAuthorizationRequirement>()
                        .OfType<RedisAuthorizationRequirement>()
                        .ToList();

                    // Check if the user has all the required permissions
                   
                    var userPrincipal = _httpContextAccessor.HttpContext.User;

                    // Create a new claim
                    var claim = new Claim("GetUser", "true", "http://www.w3.org/2001/XMLSchema#string");

                    // Add the claim to the current user's identity
                    ((ClaimsIdentity)userPrincipal.Identity).AddClaim(claim);
                    
                    var userClaims = _httpContextAccessor.HttpContext.User.Claims;
                    
                 //   if (requiredPermissions.All(requiredPermission =>
                  //      userClaims.Any(claim =>
                 //           claim.Type == requiredPermission )))
                 
                    {
                        // If all required permissions are present, succeed the requirement
                        context.Succeed(requirement);
                        return;
                    }
                }
            }

            // If Redis key not found, claims are invalid, or permissions are null, fail the requirement
            context.Fail();
        }



        private string GetRedisKey()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

            // Find the claim with type "redisKey"
            var redisKeyClaim = claimsIdentity?.FindFirst("redisKey");

            if (redisKeyClaim?.Value != null) return redisKeyClaim.Value;
            throw new ArgumentNullException();

            // Return null or throw an exception if the claim is not found
        }


        public class Permission
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class Limitation
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class RedisClaimsPayload
        {
            public List<Permission> Permissions { get; set; }
            public List<Limitation> Limitations { get; set; }
        }
    }
}