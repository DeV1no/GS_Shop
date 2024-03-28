namespace GS_Shop_UserManagement.Domain.Common;

public class BaseEntity
{
    public BaseEntity()
    {
        DateCreated = DateTime.Now;
    }

    public int Id { get; set; }
    public DateTime DateCreated { get; }
    public string CreatedBy { get; } = string.Empty;
    public DateTime LastModifiedDate { get; set; }
    public string LastModifiedBy { get; } = string.Empty;
}