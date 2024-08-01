using GS_Shop_UserManagement.Domain.Common;

namespace GS_Shop_UserManagement.Domain.Entities;

public class UserClaimLimitation : BaseEntity
{
    public string ClaimLimitationValue { get; set; } = string.Empty;
    public string LimitedIds { get; set; } = string.Empty;
    public string LimitationField { get; set; } = string.Empty;

    public string? Action { get; set; }
    public User User { get; set; } = new User();
    public int UserId { get; set; }

}