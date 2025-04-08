namespace GS_Shop.Home.Ui.Models;

public class LoginResponse
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public List<UserClaim>? UserClaim { get; set; }
    public List<UserClaimLimitation>? UserClaimLimitation { get; set; }
}

public class UserClaim
{
    public string? ClaimType { get; set; }
}

public class UserClaimLimitation
{
    public string? ClaimLimitationValue { get; set; }
    public string? LimitedIds { get; set; }
    public string? LimitationField { get; set; }
}
