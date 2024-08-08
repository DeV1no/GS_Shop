using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using GS_Shop_UserManagement.Application.DTOs.RedisClaims;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Infrastructure.Redis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GS_Shop_UserManagement.Persistence.SmartLimit.Service;

internal class SmartLimitationService<TEntity>(
    GSShopUserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IRedisCacheService redisCacheService)
    : ISmartLimitationService<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetLimitedEntitiesQueryAsync()
    {
        return LimitedEntitiesByActionQueryAsync(ClaimLimitationActionEnum.Get);
    }

    public async Task<TEntity> UpdateLimitationAsync(TEntity entity)
    {
        var entityId = GetEntityId(entity);

        // Fetch the entities from the database
        var limitedEntities = LimitedEntitiesByActionQueryAsync(ClaimLimitationActionEnum.Put);

        // Check if the user has access to update the entity by its ID
        var isEntityExistOrAccessed = limitedEntities.Any(e => GetEntityId(e) == entityId);

        if (!isEntityExistOrAccessed)
        {
            throw new UnauthorizedAccessException("User does not have access to update this entity.");
        }

        // Proceed with the update operation
        dbContext.Update(entity);
        await dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteLimitationAsync(int id)
    {
        // Fetch the IDs of the entities that the user has access to
        var entityIds = await LimitedEntitiesByActionQueryAsync(ClaimLimitationActionEnum.Delete)
            .Select(e => GetEntityDeleteId(e))
            .ToListAsync();

        // Check if the given ID exists in the fetched entity IDs
        if (!entityIds.Contains(id))
        {
            throw new UnauthorizedAccessException("User does not have access to delete this entity.");
        }

        var entity = await dbContext.Set<TEntity>().FindAsync(id)
                     ?? throw new Exception("Entity not found.");
        dbContext.Set<TEntity>().Remove(entity);
        await dbContext.SaveChangesAsync();

        return entity;
    }


    private IQueryable<TEntity> LimitedEntitiesByActionQueryAsync(ClaimLimitationActionEnum action)
    {
        //ClaimLimitationActionEnum? action= ClaimLimitationActionEnum.Get

        var entities = dbContext.Set<TEntity>();

        var user = httpContextAccessor.HttpContext?.User;
        if (user is null)
            throw new UnauthorizedAccessException();
        var redisKey = user.Claims.FirstOrDefault(x => x.Type == "redisKey")?.Value;
        if (redisKey is null)
            throw new UnauthorizedAccessException();
        var redisData = redisCacheService.Get(redisKey);
        var userClaims = JsonConvert.DeserializeObject<RedisClaims>(redisData);
        if (userClaims is null)
            return entities;
        userClaims.Limitations = userClaims.Limitations.Where(x =>
        {
            var value = x.Value;
            if (value.Contains("$&"))
            {
                // Split by the delimiter "$&"
                var parts = value.Split(new string[] {"$&"}, StringSplitOptions.None);

                // Check if the part after "$&" contains "Get"
                return parts.Length > 1 && parts[1].Contains("Get");
            }
            else
            {
                // If there is no "$&" delimiter, include the claim
                return true;
            }
        }).ToList();
        var entityType = typeof(TEntity);
        var entityName = entityType.Name;
        var limitationClaimsRedisType = GetLimitationClaims(userClaims, entityName);
        if (limitationClaimsRedisType is null)
            return entities;
        var limitationField = GetLimitationFiled(limitationClaimsRedisType);
        var actionParameter = GetActionParameter(limitationClaimsRedisType);
        if (!string.Equals(actionParameter, action.ToString(), StringComparison.CurrentCultureIgnoreCase))
            return Enumerable.Empty<TEntity>().AsQueryable();
        var limitationClaims = RemoveLastClaimParam(limitationClaimsRedisType, limitationField);
        if (string.IsNullOrEmpty(entityName))
            throw new InvalidOperationException($"No limitation tag found for entity type {entityType.Name}.");
        var limitationClaimToIntList = limitationClaims.Value.Split(',')
            .Select(int.Parse)
            .ToList();
        // Build expression for Id property
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = Expression.Property(parameter, limitationField);
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

    private string GetLimitationFiled(Limitation limitationClaims)
    {
        // Remove curly braces
        var cleanClaims = limitationClaims.Value.Trim('{', '}');

        // Split the string by commas
        var parts = cleanClaims.Split(',');

        // Access the last element
        var lastParameter = parts[^1].Trim(); // Using index from end operator and trimming any whitespace

        // Split the last parameter by the delimiter "$&"
        var subParts = lastParameter.Split(new string[] {"$&"}, StringSplitOptions.None);

        // Return the part before "$&", which is "Id"
        return subParts[0].Trim();
    }

    private string GetActionParameter(Limitation limitationClaims)
    {
        // Remove curly braces
        var cleanClaims = limitationClaims.Value.Trim('{', '}');

        // Split the string by commas
        var parts = cleanClaims.Split(',');

        // Access the last element
        var lastParameter = parts[^1].Trim(); // Using index from end operator and trimming any whitespace

        // Split the last parameter by the delimiter "$&"
        var subParts = lastParameter.Split(new string[] {"$&"}, StringSplitOptions.None);

        // Return the part after "$&", which is "Put"
        return subParts.Length > 1 ? subParts[1].Trim() : string.Empty;
    }


    private Limitation? GetLimitationClaims(RedisClaims userClaims, string limitationTag)
    {
        // Find all claims with a name ending with "Limitation" in the user's claims
        // return userClaims.(c => string.Equals(c.Type, limitationTag, StringComparison.CurrentCultureIgnoreCase));        return userClaims.(c => string.Equals(c.Type, limitationTag, StringComparison.CurrentCultureIgnoreCase));
        return userClaims.Limitations.FirstOrDefault(c =>
            string.Equals(c.Type, limitationTag, StringComparison.CurrentCultureIgnoreCase));
    }

    private int GetEntityId(TEntity entity)
    {
        // Assuming the entity has an 'Id' property
        var property = typeof(TEntity).GetProperty("Id");
        return property == null
            ? throw new InvalidOperationException("Entity must have an 'Id' property.")
            : (int) (property.GetValue(entity) ?? throw new InvalidOperationException());
    }

    private static int GetEntityDeleteId(TEntity entity)
    {
        // Assuming the entity has an 'Id' property
        var property = typeof(TEntity).GetProperty("Id");
        if (property == null)
        {
            throw new InvalidOperationException("Entity must have an 'Id' property.");
        }

        return (int) (property.GetValue(entity) ?? throw new InvalidOperationException());
    }

    private Claim RemoveLastClaimParam(Limitation limitationClaims, string limitationField)
    {
        var claimValue = limitationClaims.Value;

        // Remove curly braces if present
        claimValue = claimValue.Trim('{', '}');

        // Split the string by commas
        var parts = claimValue.Split(',');

        // Access the last element and check if it contains "$&"
        var lastParameter = parts[^1].Trim();
        if (lastParameter.Contains("$&"))
        {
            // Split the last parameter by the delimiter "$&"
            var subParts = lastParameter.Split(new string[] {"$&"}, StringSplitOptions.None);

            // Reconstruct the string without the "$&" part
            parts[^1] = subParts[0].Trim();
        }

        // Remove the limitationField (e.g., "Id") from the parts array
        parts = parts.Where(p => !p.Trim().Equals(limitationField, StringComparison.OrdinalIgnoreCase)).ToArray();

        // Join the parts back into a single string
        var updatedClaimValue = string.Join(",", parts);

        // Create a new Claim object with the updated value
        var updatedClaim = new Claim(limitationClaims.Type, updatedClaimValue, limitationClaims.ValueType,
            limitationClaims.Issuer);

        return updatedClaim;
    }
}