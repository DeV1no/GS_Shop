﻿using GS_Shop_UserManagement.Domain.Common;
using Microsoft.AspNetCore.Identity;


namespace GS_Shop_UserManagement.Domain.Entities;

public class User : IdentityUser<int>, IBaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IList<Role> Roles { get; set; } = new List<Role>();
    public IList<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public string CreatedBy { get; } = string.Empty;
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; } = string.Empty;
    public IList<UserClaimLimitation> UserClaimLimitations { get; set; } = new List<UserClaimLimitation>();
    public FileDetails ProfilePicture { get; set; } = new FileDetails();
    public int? ProfilePictureId { get; set; }
    public string? ProfilePicturePath { get; set; }
}
