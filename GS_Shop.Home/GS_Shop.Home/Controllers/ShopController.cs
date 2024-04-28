using GS_Shop.Home.Services.DTOs.Shop;
using GS_Shop.Home.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop.Home.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly IShopService _service;

    public ShopController(IShopService service)
    {
        _service = service;
    }

    [HttpGet("{name}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "GetShopPolicy")]
    public async Task<IActionResult> GetShopByName(string name)
    {
        var shop = await _service.GetShopDetail(name);
        return Ok(shop);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShop(CreateShopDto dto)
    {
        var shopName = await _service.AddShop(dto);
        return Ok(shopName);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateShop(UpdateShopDto dto)
    {
        var shopName = await _service.UpdateShop(dto);
        return Ok(shopName);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteShop(string name)
    {
        return Ok(await _service.DeleteShop(name));
    }
}