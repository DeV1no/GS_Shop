using AutoMapper;
using GS_Shop.Home.Core.Entities;
using GS_Shop.Home.Services.DTOs.Shop;
using GS_Shop.Home.Services.Helper.SmartLimit;
using GS_Shop.Home.Services.IServices;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace GS_Shop.Home.Services.Services;

public class ShopService:IShopService
{
    private readonly IDistributedCache _redisCache;
    private readonly IMapper _mapper;
    public ShopService(IDistributedCache redisCache, IMapper mapper)
    {
        _redisCache = redisCache;
        _mapper = mapper;
    }
    public Task<ShopListDto> GetShopList()
    {
        throw new NotImplementedException();
    }

    public async Task<ShopDetailDto?> GetShopDetail(string name)
    {
       var shop =await _redisCache.GetStringAsync(name);
      // var shop = await _smartLimit.GetStringAsync(name);
        return shop == null ? null : JsonConvert.DeserializeObject<ShopDetailDto>(shop);
    }

    public async Task<string> AddShop(CreateShopDto dto)
    {
        await _redisCache.SetStringAsync(dto.Name,
            JsonConvert.SerializeObject(dto));
        return dto.Name;
    }

    public async Task<string> UpdateShop(UpdateShopDto dto)
    {
        await _redisCache.SetStringAsync(dto.Name,
            JsonConvert.SerializeObject(dto));
        return dto.Name;
    }

    public async Task<bool> DeleteShop(string name)
    {
        await _redisCache.RemoveAsync(name);
        return true;
    }
}