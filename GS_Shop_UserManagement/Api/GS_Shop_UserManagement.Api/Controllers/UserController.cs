using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop_UserManagement.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "GetUserListPolicy")]
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var allocationList = await _mediator.Send(new GetAllUserRequest());
        return Ok(allocationList);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var command = new RegisterUserCommand { RegisterUserDto = dto };
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> login([FromBody] LoginDto dto)
    {
        var command = new LoginUserCommand() { LoginDto = dto };
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}