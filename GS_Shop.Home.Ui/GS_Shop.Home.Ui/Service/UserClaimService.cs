using System.Text.Json;
using GS_Shop.Home.Ui.Models;
using Microsoft.JSInterop;

namespace GS_Shop.Home.Ui.Service;


public class UserClaimService
{
    private readonly IJSRuntime js;
    private List<UserClaim> _cachedClaims;

    public UserClaimService(IJSRuntime js)
    {
        this.js = js;
    }
    private async Task<List<UserClaim>> GetClaimsAsync()
    {
        if (_cachedClaims != null)
            return _cachedClaims;

        var json = await js.InvokeAsync<string>("localStorage.getItem", "userClaim");
        if (string.IsNullOrWhiteSpace(json))
            return new List<UserClaim>();

        _cachedClaims = JsonSerializer.Deserialize<List<UserClaim>>(json) ?? new List<UserClaim>();
        return _cachedClaims;
    }

    public async Task<bool> HasAsync(string claim) =>
        await HasAnyAsync([claim]);

    public async Task<bool> HasAnyAsync(IEnumerable<string> claims)
    {
        var userClaims = await GetClaimsAsync();
        return userClaims.Any(c => claims.Contains(c.ClaimType));
    }

    public async Task<bool> HasAllAsync(IEnumerable<string> claims)
    {
        var userClaims = await GetClaimsAsync();
        return claims.All(rc => userClaims.Any(uc => uc.ClaimType == rc));
    }

    public void ClearCache() => _cachedClaims = null;
}

