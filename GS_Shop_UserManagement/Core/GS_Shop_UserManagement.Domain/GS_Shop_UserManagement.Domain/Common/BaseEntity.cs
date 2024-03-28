namespace GS_Shop_UserManagement.Domain.Common;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string CreatedBy { get; } = string.Empty;
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; } = string.Empty;
}