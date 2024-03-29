using GS_Shop_UserManagement.Domain.Entities;

namespace GS_Shop_UserManagement.Application.Contracts.Persistence;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsUserExistByUserAndEmail(string userName, string email);
}