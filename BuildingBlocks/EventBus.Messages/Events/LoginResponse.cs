namespace EventBus.Messages.Events;

public class LoginResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    
    public IList<UserClaimLimitationDto> ClaimLimitation { get; set; } = new List<UserClaimLimitationDto>();
    public IList<UserClaimDto> Claim { get; set; } = new List<UserClaimDto>();
}

