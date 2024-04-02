using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Persistence.SmartLimit.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GS_Shop_UserManagement.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ISmartLimitationService<User> _smartLimitationService;
    private readonly GSShopUserManagementDbContext _context;
    public UserRepository(GSShopUserManagementDbContext context, UserManager<User> userManager, ISmartLimitationService<User> smartLimitationService) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _smartLimitationService = smartLimitationService;
    }

    public async Task<bool> IsUserExistByUserAndEmail(string userName, string email)
        => await _context.Users.AnyAsync(x => x.UserName == userName || x.Email == email);

    public async Task<User> GetUserByUserAndPassword(string password, string userName)
    {
        var user = await _context.Users
            .Include(x => x.UserClaims)
            .Include(x => x.Roles)
            .Include(x => x.UserClaimLimitations)
            .SingleOrDefaultAsync(x => x.NormalizedUserName == userName.ToUpper())
                   ?? throw new Exception("User Not Found");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect)
            throw new Exception("User Not Found");
        return user;
    }

    public async Task<bool> Delete(int id)
    {
        await _smartLimitationService.DeleteLimitationAsync(id);
        return true;
    }

    public new async Task<IReadOnlyList<User>> GetAll()
        => await _smartLimitationService.GetLimitedEntitiesQueryAsync().ToListAsync();


    public new Task<User> Update(User entity)
        => _smartLimitationService.UpdateLimitationAsync(entity);


}