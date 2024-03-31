using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GS_Shop_UserManagement.Domain.Enums;

namespace GS_Shop_UserManagement.Persistence.SmartLimit.Service;

public class SmartLimitationService<TEntity> : ISmartLimitationService<TEntity> where TEntity : class
{
    private readonly GSShopUserManagementDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SmartLimitationService(GSShopUserManagementDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IQueryable<TEntity>> GetLimitedEntitiesQueryAsync()
    {
        var userClaims = (_httpContextAccessor.HttpContext?.User) ?? throw new InvalidOperationException("No user claims found.");
        var entityType = typeof(TEntity);
        var limitationTag = GetLimitationTag(entityType);
        var limitationClaims = GetLimitationClaims(userClaims, limitationTag); // Get all limitation claims dynamically

        if (string.IsNullOrEmpty(limitationTag))
        {
            throw new InvalidOperationException($"No limitation tag found for entity type {entityType.Name}.");
        }

        var limitationClaimToIntList = limitationClaims.Value.Split(',')
            .Select(int.Parse)
            .ToList();

        var limitedEntities = await _dbContext.Set<TEntity>().ToListAsync();

        // Filter entities based on the limitation IDs
        var result = limitedEntities.Where(entity => limitationClaimToIntList.Contains(GetEntityId(entity))).AsQueryable();

        return result;
    }

    private Claim GetLimitationClaims(ClaimsPrincipal userClaims, string limitationTag)
    {
        // Find all claims with a name ending with "Limitation" in the user's claims
        return userClaims.Claims.First(c => c.Type.EndsWith("Limitation") && c.Type == limitationTag);

    }

    private IEnumerable<int> GetLimitationIds(ClaimsPrincipal userClaims, IEnumerable<string> limitationClaims)
    {
        // Extract limitation IDs from the claims in the token
        var limitationIds = new List<int>();
        foreach (var claim in userClaims.Claims)
        {
            if (limitationClaims.Contains(claim.Type))
            {
                var ids = claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);
                limitationIds.AddRange(ids);
            }
        }
        return limitationIds.Distinct();
    }

    private string GetLimitationTag(Type entityType)
    {
        // Get the limitation tag associated with the entity type
        var attribute = entityType.GetCustomAttributes(typeof(SmartLimitTagAttribute), inherit: true)
            .FirstOrDefault() as SmartLimitTagAttribute;
        return attribute?.LimitationTag!;
    }

    private int GetEntityId(TEntity entity)
    {
        // Assuming the entity has an 'Id' property
        var property = typeof(TEntity).GetProperty("Id");
        if (property == null)
        {
            throw new InvalidOperationException("Entity must have an 'Id' property.");
        }
        return (int)property.GetValue(entity)!;
    }
}
