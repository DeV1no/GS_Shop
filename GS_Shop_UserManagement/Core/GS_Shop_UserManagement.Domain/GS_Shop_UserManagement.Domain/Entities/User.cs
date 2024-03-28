using System.ComponentModel.DataAnnotations;
using GS_Shop_UserManagement.Domain.Common;


namespace GS_Shop_UserManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public IList<Role> Roles { get; set; } = new List<Role>();
        public IList<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    }
}