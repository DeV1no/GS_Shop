namespace GS_Shop_UserManagement.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : class
{
    Task<T?> Get(int id);
    Task<IReadOnlyList<T>> GetAll();
    Task<T> Add(T entity);
    Task<T> Update(T entity);
    Task Delete(T entity);
    Task<bool> Exist(int id);
}