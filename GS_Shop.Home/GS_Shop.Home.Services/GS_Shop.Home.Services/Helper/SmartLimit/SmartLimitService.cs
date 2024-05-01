using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace GS_Shop.Home.Services.Helper.SmartLimit;

    public class SmartLimitService<TEntity> : ISmartLimitService<TEntity> where TEntity : class
    {
        private readonly IDistributedCache _redisDb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SmartLimitService(IDistributedCache redisDb, IHttpContextAccessor httpContextAccessor)
        {
            _redisDb = redisDb;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<TEntity> GetLimitedEntitiesQueryAsync()
        {
            var serializedEntities = _redisDb.GetString("limited_entities");
            if (!string.IsNullOrEmpty(serializedEntities))
            {
                var entities = JsonConvert.DeserializeObject<TEntity[]>(serializedEntities);
                return entities.AsQueryable();
            }
            else
            {
                return Enumerable.Empty<TEntity>().AsQueryable();
            }
        }

        public async Task<string?> GetStringAsync(string name)
        {
            var hasUserAccess = CheckUserLimitation(name);
            if (hasUserAccess is false)
                throw new Exception("You Dont Have Access");
            return await _redisDb.GetStringAsync(name);
        }


        public async Task<TEntity> UpdateLimitationAsync(TEntity entity)
        {
            // Implementation for updating limitation in distributed cache
            throw new NotImplementedException();
        }

        public async Task<TEntity> DeleteLimitationAsync(int id)
        {
            // Implementation for deleting limitation from distributed cache
            throw new NotImplementedException();
        }

        private Claim? GetLimitationClaims(ClaimsPrincipal userClaims, string limitationTag)
        {
            return userClaims.Claims.FirstOrDefault(c => string.Equals(c.Type, limitationTag, StringComparison.CurrentCultureIgnoreCase));
        }

        private int GetEntityId(TEntity entity)
        {
            // Assuming the entity has an 'Id' property
            // You may need to adjust this based on your entity structure
            var property = entity.GetType().GetProperty("Id");
            if (property == null || !(property.GetValue(entity) is int id))
            {
                throw new InvalidOperationException("Entity must have an 'Id' property of type int.");
            }
            return id;
        }

        private Claim RemoveLastClaimParam(Claim limitationClaims, string limitationField)
        {
            // Implementation for removing the last claim parameter
            throw new NotImplementedException();
        }

        private bool CheckUserLimitation(string name)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            if (userClaims is null)
                return true;
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;
            var limitationClaims = GetLimitationClaims(userClaims, entityName);
            if (limitationClaims is null)
                return true;
            var limitationField = GetLimitationFiled(limitationClaims);
            limitationClaims = RemoveLastClaimParam(limitationClaims, limitationField);
            if (string.IsNullOrEmpty(entityName))
                throw new InvalidOperationException($"No limitation tag found for entity type {entityType.Name}.");
            var limitationClaimToIntList = limitationClaims.Value.Split(',')
                .Select(int.Parse)
                .ToList();
            return true;
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

    }

