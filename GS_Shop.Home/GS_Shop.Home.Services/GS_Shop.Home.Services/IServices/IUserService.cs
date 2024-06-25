using EventBus.Messages.Events;
using GS_Shop.Home.Services.DTOs;
using GS_Shop.Home.Services.DTOs.User;
using MassTransit;

namespace GS_Shop.Home.Services.IServices;

public interface IUserService
{
    public Task<LoginResponseDto> Login(LoginEvent login);
    public Task<RegisterResponse> Register(RegisterEvent register);
}