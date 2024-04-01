using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using GS_Shop_UserManagement.Domain.Enums;

namespace GS_Shop_UserManagement.Persistence.SmartLimit.Service;

public class SmartLimitationService<TEntity>(GSShopUserManagementDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    : ISmartLimitationService<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetLimitedEntitiesQueryAsync()
    {
        var entities = dbContext.Set<TEntity>();
        var userClaims = httpContextAccessor.HttpContext?.User;
        if (userClaims is null)
            return entities;

        var entityType = typeof(TEntity);
        var limitationTag = GetLimitationTag(entityType);
        var limitationClaims = GetLimitationClaims(userClaims, limitationTag);
        if (limitationClaims is null)
            return entities;
        if (string.IsNullOrEmpty(limitationTag))
            throw new InvalidOperationException($"No limitation tag found for entity type {entityType.Name}.");
        var limitationClaimToIntList = limitationClaims.Value.Split(',')
            .Select(int.Parse)
            .ToList();
        // Build expression for Id property
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = Expression.Property(parameter, "Id");
        // Compose the where clause dynamically
        var containsMethod = typeof(Enumerable).GetMethods()
            .Single(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(int));
        var constant = Expression.Constant(limitationClaimToIntList);
        var containsExpression = Expression.Call(containsMethod, constant, property);
        var whereExpression = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);
        // Apply the where clause
        var result = entities.Where(whereExpression);
        return result;
    }




    private Claim? GetLimitationClaims(ClaimsPrincipal userClaims, string limitationTag)
    {
        // Find all claims with a name ending with "Limitation" in the user's claims
        return userClaims.Claims.FirstOrDefault(c => c.Type.EndsWith("Limitation") && c.Type == limitationTag);

    }

    private string GetLimitationTag(Type entityType)
    {
        // Get the limitation tag associated with the entity type
        var attribute = entityType.GetCustomAttributes(typeof(SmartLimitTagAttribute), inherit: true)
            .FirstOrDefault() as SmartLimitTagAttribute;
        return attribute?.LimitationTag!;
    }
}
