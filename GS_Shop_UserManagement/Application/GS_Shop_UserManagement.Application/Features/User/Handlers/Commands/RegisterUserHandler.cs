using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public RegisterUserHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}