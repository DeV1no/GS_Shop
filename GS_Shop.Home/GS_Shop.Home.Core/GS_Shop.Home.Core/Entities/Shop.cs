namespace GS_Shop.Home.Core.Entities;

public class Shop
{
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public Category Category { get; set; } = new Category();
    public int CategoryId { get; set; }
}