using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;
using GS_Shop_UserManagement.Persistence.SmartLimit.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GS_Shop_UserManagement.Persistence.Repositories;

public class UserRepository(GSShopUserManagementDbContext context,
    UserManager<User> userManager,
    ISmartLimitationService<User> smartLimitationService, IMongoLoggerService mongoLogger)
    : GenericRepository<User>(context), IUserRepository
{
    public async Task<bool> IsUserExistByUserAndEmail(string userName, string email)
        => await context.Users.AnyAsync(x => x.UserName == userName || x.Email == email);

    public async Task<User> GetUserByUserAndPassword(string password, string userName)
    {
        var user = await context.Users
            .Include(x => x.UserClaims)
            .Include(x => x.Roles)
            .Include(x => x.UserClaimLimitations)
            .SingleOrDefaultAsync(x => x.NormalizedUserName == userName.ToUpper())
                   ?? throw new Exception("User Not Found");

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect)
            throw new Exception("User Not Found");
        return user;
    }

    public async Task<int> RegisterUser(User user, string password)
    {
        // Set password
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception("Failed to create user. Error: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        var requestJson = JsonConvert.SerializeObject(user);
        mongoLogger.AddLog(new AddLogDto
        {
            UserId = user.Id.ToString(),
            EndPointType = EntityEndPointType.Post,
            EntityType = "User",
            RequestDeserialized = requestJson,
            ResponseDeserialized = user.Id.ToString()
        });
        return user.Id;
    }

    public async Task<bool> Delete(int id)
    {
        await smartLimitationService.DeleteLimitationAsync(id);
        mongoLogger.AddLog(new AddLogDto
        {
            EndPointType = EntityEndPointType.Delete, 
            EntityType = "User",
            RequestDeserialized = id.ToString(),
            ResponseDeserialized = "true"
        });
        return true;
    }

    public new async Task<IReadOnlyList<User>> GetAll()
        => await smartLimitationService.GetLimitedEntitiesQueryAsync().ToListAsync();


    public new async Task<User> Update(User entity)
    {
        var requestJson = JsonConvert.SerializeObject(entity);
        mongoLogger.AddLog(new AddLogDto
        {
            UserId = entity.Id.ToString(),
            EndPointType = EntityEndPointType.Put,
            EntityType = "User",
            RequestDeserialized = requestJson,
            ResponseDeserialized = entity.Id.ToString()
        });
        return await smartLimitationService.UpdateLimitationAsync(entity);
    }



}