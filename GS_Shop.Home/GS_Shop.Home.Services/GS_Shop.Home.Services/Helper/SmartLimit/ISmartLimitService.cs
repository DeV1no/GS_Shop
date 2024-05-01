namespace GS_Shop.Home.Services.Helper.SmartLimit;

public interface ISmartLimitService<TEntity>
{
    public IQueryable<TEntity> GetLimitedEntitiesQueryAsync();
    public Task<string?> GetStringAsync(string name);
    public Task<TEntity> UpdateLimitationAsync(TEntity entity);
    public Task<TEntity> DeleteLimitationAsync(int id);
}