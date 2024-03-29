using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GS_Shop_UserManagement.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly GSShopUserManagementDbContext _context;
    public UserRepository(GSShopUserManagementDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsUserExistByUserAndEmail(string userName, string email)
        => await _context.Users.AnyAsync(x => x.UserName == userName || x.Email == email);
}