using GS_Shop_UserManagement.Domain.Common;

namespace GS_Shop_UserManagement.Domain.Entities;

public class UserClaim : BaseEntity
{
    // public string ClaimValue { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public User User { get; set; } = new User();
    public int UserId { get; set; }
}