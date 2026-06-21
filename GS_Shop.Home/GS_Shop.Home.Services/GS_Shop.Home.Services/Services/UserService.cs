using EventBus.Messages.Events;
using GS_Shop.Home.Services.DTOs.User;
using GS_Shop.Home.Services.IServices;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace GS_Shop.Home.Services.Services;

public class UserService : IUserService
{
    private readonly IRequestClient<LoginEvent> _requestClient;
    private readonly IRequestClient<RegisterEvent> _registerRequestClient;
    private readonly IRequestClient<UserListEvent> _userListRequestClient;
    private readonly IRequestClient<UserListPublicEvent> _userListPublicRequestClient;
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IRequestClient<LoginEvent> requestClient,
        IOptions<JwtSettings> jwtSettings,
        IRequestClient<RegisterEvent> registerRequestClient,
        IRequestClient<UserListEvent> userListRequestClient,
        IRequestClient<UserListPublicEvent> userListPublicRequestClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _requestClient = requestClient;
        _registerRequestClient = registerRequestClient;
        _userListRequestClient = userListRequestClient;
        _userListPublicRequestClient = userListPublicRequestClient;
        _jwtSettings = jwtSettings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponseDto> Login(LoginEvent login)
    {
        var response = await _requestClient.GetResponse<LoginResponse>(login);
        if (response is null)
            throw new Exception();

        return new LoginResponseDto
        {
            Id = response.Message.Id,
            UserName = response.Message.UserName!,
            Email = response.Message.Email!,
            Token = response.Message.Token,
            ExpiresAt = response.Message.ExpiresAt,
            UserClaim = response.Message.Claim,
            UserClaimLimitation = response.Message.ClaimLimitation
        };
    }

    public async Task<RegisterResponse> Register(RegisterEvent register)
    {
        var response = await _registerRequestClient.GetResponse<RegisterResponse>(register);
        if (response is null)
            throw new Exception();
        return response.Message;
    }

    public async Task<UserListResponse> GetUserList()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found in token");

        var userIdInt=int.Parse(userId);
        // Optionally, you can use this userId in your logic or pass it to the UserListEvent
        var userListEvent = new UserListEvent
        {
            UserId = userIdInt // (Assuming you add this property)
        };
        var response = await _userListRequestClient
            .GetResponse<UserListResponse>(userListEvent);
        if (response == null)
            throw new Exception();
        return response.Message;
    }
    
    public async Task<List<UserListPublicResponse>> GetUserListPublic()
    {
        var userListEvent = new UserListPublicEvent
        {
        };
        var response = await _userListPublicRequestClient
            .GetResponse<UserListPublicResponseList>(userListEvent);
        if (response == null)
            throw new Exception();

        return response.Message.Users;
    }
}