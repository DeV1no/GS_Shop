using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Queries;

public class GetAllUserPublicHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetAllUserPublicRequest, List<UserListDto>>
{
    public async Task<List<UserListDto>> Handle(GetAllUserPublicRequest request, CancellationToken cancellationToken)
    {
        var userList = await repository.GetAllNoLimit();
        return mapper.Map<List<UserListDto>>(userList);
    }
}