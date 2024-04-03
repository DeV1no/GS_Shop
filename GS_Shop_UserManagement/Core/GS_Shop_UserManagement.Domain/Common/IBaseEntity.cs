namespace GS_Shop_UserManagement.Domain.Common;

public interface IBaseEntity
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string CreatedBy { get; } 
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; }
}