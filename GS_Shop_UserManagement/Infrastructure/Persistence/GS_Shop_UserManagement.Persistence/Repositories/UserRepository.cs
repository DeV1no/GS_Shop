using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;

namespace GS_Shop_UserManagement.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly GSShopUserManagementDbContext _context;
    public UserRepository(GSShopUserManagementDbContext context) : base(context)
    {
        _context = context;
    }
}