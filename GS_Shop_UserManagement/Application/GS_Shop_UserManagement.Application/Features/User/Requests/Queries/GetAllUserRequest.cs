using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Queries;

public class GetAllUserRequest : IRequest<List<UserListDto>>
{

}