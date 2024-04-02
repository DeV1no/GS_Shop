using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return _userRepository.Delete(request.Id);
    }
}