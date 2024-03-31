using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MediatR;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Queries;

public class GetAllUserHandler : IRequestHandler<GetAllUserRequest, List<UserListDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetAllUserHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<UserListDto>> Handle(GetAllUserRequest request, CancellationToken cancellationToken)
    {
        var userList = await _repository.GetAll();
        return _mapper.Map<List<UserListDto>>(userList);
    }
}