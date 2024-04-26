namespace GS_Shop.Home.Core.Entities;

public class Category
{
    public string Name { get; set; } = string.Empty;
    public List<Shop> Shops { get; set; } = new List<Shop>();
}