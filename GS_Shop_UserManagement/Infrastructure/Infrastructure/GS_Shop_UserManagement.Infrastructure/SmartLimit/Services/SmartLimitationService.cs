using GS_Shop_UserManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GS_Shop_UserManagement.Infrastructure.SmartLimit.Services;

public class SmartLimitationService : ISmartLimitationService
{
    private readonly GSShopUserManagementDbContext _dbContext;
    private readonly IEnumerable<string> _limitationClaims;

    public SmartLimitationService(GSShopUserManagementDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _limitationClaims = LimitationClaimLoader.LoadLimitationClaims(configuration);
    }

    public async Task<IEnumerable<int>> GetLimitedIdsAsync(int userId, string limitationClaim)
    {
        if (!_limitationClaims.Contains(limitationClaim))
        {
            throw new ArgumentException($"Limitation claim '{limitationClaim}' is not valid.", nameof(limitationClaim));
        }

        var userClaimLimitations = await _dbContext.UserLimitationClaims
            .Where(ucl => ucl.UserId == userId && ucl.ClaimLimitationValue == limitationClaim)
            .ToListAsync();

        var limitedIds = userClaimLimitations
            .SelectMany(ucl => ucl.LimitedIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(int.Parse)
            .Distinct();

        return limitedIds;
    }
}
