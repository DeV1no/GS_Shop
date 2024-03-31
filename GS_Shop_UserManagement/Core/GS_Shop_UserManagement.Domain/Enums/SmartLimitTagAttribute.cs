namespace GS_Shop_UserManagement.Domain.Enums;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SmartLimitTagAttribute : Attribute
{
    public string LimitationTag { get; }

    public SmartLimitTagAttribute(string limitationTag)
    {
        LimitationTag = limitationTag;
    }
}