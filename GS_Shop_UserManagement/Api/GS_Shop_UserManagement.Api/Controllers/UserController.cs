﻿using GS_Shop_UserManagement.Application.Attributes;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using GS_Shop_UserManagement.Application.Features.User.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop_UserManagement.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    //[Authorize(AuthenticationSchemes = "Bearer", Policy = "GetUserListPolicy")]
    [Auth("GetUserListPolicy")]
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll()
    {
        var allocationList = await mediator.Send(new GetAllUserRequest());
        return Ok(allocationList);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var command = new RegisterUserCommand { RegisterUserDto = dto };
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var command = new LoginUserCommand() { LoginDto = dto };
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "UpdateUserPolicy")]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto dto)
    {
        var command = new UpdateUserCommand { UpdateUserDto = dto };
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "DeleteUserPolicy")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand { Id = id };
        var response = await mediator.Send(command);
        return Ok(response);
    }
}