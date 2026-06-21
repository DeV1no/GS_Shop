using AutoMapper;
using EventBus.Messages.Events;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Domain.Entities;
using UserClaimDto = GS_Shop_UserManagement.Application.DTOs.User.UserClaimDto;
using UserClaimLimitationDto = GS_Shop_UserManagement.Application.DTOs.User.UserClaimLimitationDto;

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
        CreateMap<UserListDto, UserListResponse>();
        CreateMap<UserListDto, UserListPublicResponse>();
    }
}