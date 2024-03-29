using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Commands;

public class RegisterUserCommand : IRequest<int>
{
    public RegisterUserDto RegisterUserDto { get; set; } = new RegisterUserDto();
}