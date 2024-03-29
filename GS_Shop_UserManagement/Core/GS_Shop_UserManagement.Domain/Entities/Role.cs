using GS_Shop_UserManagement.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace GS_Shop_UserManagement.Domain.Entities;

public class Role : IdentityRole<int>
{
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new List<User>();
    public DateTime DateCreated { get; set; }
    public string CreatedBy { get; } = string.Empty;
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; } = string.Empty;
}