using AutoMapper;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Domain.Entities;

namespace GS_Shop_UserManagement.Application.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterUserDto, User>()
            .ForMember(x => x.Id, opt => opt.Ignore());
        CreateMap<User, UserListDto>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<UserClaimLimitation, UserClaimLimitationDto>();
        CreateMap<UserClaim, UserClaimDto>();
    }
}