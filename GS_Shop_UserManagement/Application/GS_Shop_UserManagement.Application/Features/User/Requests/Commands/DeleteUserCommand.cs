using GS_Shop_UserManagement.Application.DTOs.User;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Requests.Commands;

public class DeleteUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}