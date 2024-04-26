using GS_Shop.Home.Services.DTOs.Shop;

namespace GS_Shop.Home.Services.IServices;

public interface IShopService
{
    Task<ShopListDto> GetShopList();
    Task<ShopDetailDto?> GetShopDetail(string name);
    Task<string> AddShop(CreateShopDto dto);
    Task<string> UpdateShop(UpdateShopDto dto);
    Task<bool> DeleteShop(string name);
}