using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using GS_Shop_UserManagement.Domain.Entities;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<Domain.Entities.User> _userManager;


    public RegisterUserHandler(IUserRepository repository, IMapper mapper, UserManager<Domain.Entities.User> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _userManager = userManager;
    }
    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isUserExist = await _repository.IsUserExistByUserAndEmail(request.RegisterUserDto.UserName, request.RegisterUserDto.Email);
        if (isUserExist)
            throw new Exception("User already exists");

        var user = _mapper.Map<Domain.Entities.User>(request.RegisterUserDto);
        // Set password
        var result = await _userManager.CreateAsync(user, request.RegisterUserDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception("Failed to create user. Error: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // addRole
        return user.Id;
    }
}