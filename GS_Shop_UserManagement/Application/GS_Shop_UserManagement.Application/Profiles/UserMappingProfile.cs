using AutoMapper;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GS_Shop_UserManagement.Application.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterUserDto, User>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}