using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Commands;

public class LoginUserCommand : IRequest<LoginResponseDto>
{
    public LoginDto LoginDto { get; set; } = new LoginDto();
}