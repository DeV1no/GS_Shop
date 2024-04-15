using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        var entityName = entityType.Name;
        var limitationClaims = GetLimitationClaims(userClaims, entityName);
        if (limitationClaims is null)
            return entities;
        var limitationField = GetLimitationFiled(limitationClaims);
        limitationClaims = RemoveLastClaimParam(limitationClaims, limitationField);
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

    private string GetLimitationFiled(Claim? limitationClaims)
    {
        // Remove curly braces
        var cleanClaims = limitationClaims!.Value.Trim('{', '}');

        // Split the string by commas
        var parts = cleanClaims.Split(',');

        // Access the last element
        var lastParameter = parts[^1].Trim(); // Using index from end operator and trimming any whitespace
        return lastParameter;

    }

    public async Task<TEntity> UpdateLimitationAsync(TEntity entity)
    {
        var entityId = GetEntityId(entity);

        // Fetch the entities from the database
        var limitedEntities = await GetLimitedEntitiesQueryAsync().ToListAsync();

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
        var entityIds = await GetLimitedEntitiesQueryAsync()
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


    private Claim? GetLimitationClaims(ClaimsPrincipal userClaims, string limitationTag)
    {
        // Find all claims with a name ending with "Limitation" in the user's claims
        return userClaims.Claims.FirstOrDefault(c => string.Equals(c.Type, limitationTag, StringComparison.CurrentCultureIgnoreCase));
    }

    private int GetEntityId(TEntity entity)
    {
        // Assuming the entity has an 'Id' property
        var property = typeof(TEntity).GetProperty("Id");
        return property == null
            ? throw new InvalidOperationException("Entity must have an 'Id' property.")
            : (int)property.GetValue(entity)!;
    }

    private static int GetEntityDeleteId(TEntity entity)
    {
        // Assuming the entity has an 'Id' property
        var property = typeof(TEntity).GetProperty("Id");
        if (property == null)
        {
            throw new InvalidOperationException("Entity must have an 'Id' property.");
        }
        return (int)property.GetValue(entity)!;
    }

    private Claim RemoveLastClaimParam(Claim limitationClaims, string limitationField)
    {
        var claimValue = limitationClaims.Value;
        var parts = claimValue.Split(',');
        claimValue = string.Join(",", parts.Take(parts.Length - 1));

        // Create a new Claim object with the updated value
        var updatedClaim = new Claim(limitationClaims.Type, claimValue, limitationClaims.ValueType, limitationClaims.Issuer);

        return updatedClaim;
    }


}
