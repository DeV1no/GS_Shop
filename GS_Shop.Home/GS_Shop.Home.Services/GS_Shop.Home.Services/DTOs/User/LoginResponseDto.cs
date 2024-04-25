using EventBus.Messages.Events;

namespace GS_Shop.Home.Services.DTOs.User;

public class LoginResponseDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public IList<UserClaimDto> UserClaim { get; set; } = new List<UserClaimDto>();
    public IList<UserClaimLimitationDto> UserClaimLimitation { get; set; } = new List<UserClaimLimitationDto>();
}