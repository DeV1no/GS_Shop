using AutoMapper;
using GS_Shop.Home.Core.Entities;
using GS_Shop.Home.Services.DTOs.Shop;

namespace GS_Shop.Home.Services.Mapper;

public class ShopMappings:Profile
{
    public ShopMappings()
    {
        CreateMap<CreateShopDto, Shop>();
        CreateMap<Shop, ShopListDto>();
        CreateMap<Shop, ShopDetailDto>();
    }
}