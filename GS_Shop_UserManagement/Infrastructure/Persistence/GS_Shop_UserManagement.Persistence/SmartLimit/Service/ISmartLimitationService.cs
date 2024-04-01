﻿namespace GS_Shop_UserManagement.Persistence.SmartLimit.Service;

public interface ISmartLimitationService<TEntity>
{
    public IQueryable<TEntity> GetLimitedEntitiesQueryAsync();
}