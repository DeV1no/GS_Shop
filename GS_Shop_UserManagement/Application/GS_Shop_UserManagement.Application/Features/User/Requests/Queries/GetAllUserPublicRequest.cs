using Amazon.Runtime.Internal;
using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Queries;

public class GetAllUserPublicRequest:IRequest<List<UserListDto>>
{
    
}