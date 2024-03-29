using System.ComponentModel.DataAnnotations;
using GS_Shop_UserManagement.Domain.Common;
using Microsoft.AspNetCore.Identity;


namespace GS_Shop_UserManagement.Domain.Entities
{
    public class User : IdentityUser<int>
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public IList<Role> Roles { get; set; } = new List<Role>();
        public IList<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; } = string.Empty;
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; } = string.Empty;
    }
}