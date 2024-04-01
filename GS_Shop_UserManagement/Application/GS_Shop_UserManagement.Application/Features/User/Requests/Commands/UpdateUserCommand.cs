using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Commands;

public class UpdateUserCommand : IRequest<int>
{
    public UpdateUserDto UpdateUserDto { get; set; } = new UpdateUserDto();
}