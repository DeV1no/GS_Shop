namespace GS_Shop_UserManagement.Infrastructure.Helpers;



public static class GetEntityAsStringHelper 
{
    public static string GetEntityAsStrings<TEntity>()
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name;
        return entityName;
    }
}
