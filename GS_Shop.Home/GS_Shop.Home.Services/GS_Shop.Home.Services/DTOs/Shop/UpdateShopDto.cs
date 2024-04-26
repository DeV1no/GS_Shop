namespace GS_Shop.Home.Services.DTOs.Shop;

public class UpdateShopDto
{
    public string Name { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public int CategoryId { get; set; }
}