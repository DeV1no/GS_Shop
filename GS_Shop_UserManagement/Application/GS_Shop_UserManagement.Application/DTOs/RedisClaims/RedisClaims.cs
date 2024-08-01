namespace GS_Shop_UserManagement.Application.DTOs.RedisClaims;

public class RedisClaims
{
    public List<Permission> Permissions { get; set; } = new List<Permission>();
    public List<object> Roles { get; set; } = new List<object>();
    public List<Limitation> Limitations { get; set; } = new List<Limitation>();
}

public class Permission
{
    public string Issuer { get; set; } = string.Empty;
    public string OriginalIssuer { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    public object Subject { get; set; } = new object();
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
}

public class Limitation
{
    public string Issuer { get; set; } = string.Empty;
    public string OriginalIssuer { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    public object Subject { get; set; } = new object();
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
}
