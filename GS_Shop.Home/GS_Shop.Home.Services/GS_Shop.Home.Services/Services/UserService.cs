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
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IRequestClient<LoginEvent> requestClient,
        IOptions<JwtSettings> jwtSettings,
        IRequestClient<RegisterEvent> registerRequestClient,
        IRequestClient<UserListEvent> userListRequestClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _requestClient = requestClient;
        _registerRequestClient = registerRequestClient;
        _userListRequestClient = userListRequestClient;
        _jwtSettings = jwtSettings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponseDto> Login(LoginEvent login)
    {
        var response = await _requestClient.GetResponse<LoginResponse>(login);
        if (response is null)
            throw new Exception();

        var jwtSecurityToken = GenerateToken(response.Message);
        return new LoginResponseDto
        {
            Id = response.Message.Id,
            UserName = response.Message.UserName!,
            Email = response.Message.Email!,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            ExpiresAt = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
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


private JwtSecurityToken GenerateToken(LoginResponse user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        claims.AddRange(user.Claim.Select(userClaim => new Claim(userClaim.ClaimType, "true")));
        // claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        claims.AddRange(user.ClaimLimitation.Select(lClaim =>
            new Claim(lClaim.ClaimLimitationValue, lClaim.LimitedIds + "," + lClaim.LimitationField)));

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes)
        );
        return jwtSecurityToken;
    }
}