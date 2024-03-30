using Microsoft.Extensions.Configuration;

using System.Text.Json;
namespace GS_Shop_UserManagement.Infrastructure.SmartLimit;

public static class LimitationClaimLoader
{
    public static IEnumerable<string> LoadLimitationClaims(IConfiguration configuration)
    {
        var limitationsFilePath = configuration.GetValue<string>("LimitationsFilePath");
        var limitationsJson = File.ReadAllText(limitationsFilePath!);
        var limitationClaims = JsonSerializer.Deserialize<LimitationClaims>(limitationsJson);

        return limitationClaims?.LimitationClaim ?? new List<string>();
    }
}

public class LimitationClaims
{
    public List<string> LimitationClaim { get; set; } = new List<string>();
}