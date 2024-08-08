using GS_Shop_UserManagement.Domain.Entities;

namespace GS_Shop_UserManagement.Persistence.SmartLimit.Service;

public interface ISmartLimitationService<TEntity>
{
    public IQueryable<TEntity> GetLimitedEntitiesQueryAsync();
    public Task<TEntity> UpdateLimitationAsync(TEntity entity);
    public Task<TEntity> DeleteLimitationAsync(int id);
}