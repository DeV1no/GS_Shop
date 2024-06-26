using EventBus.Messages.Events;
using GS_Shop.Home.Services.DTOs.User;
using GS_Shop.Home.Services.IServices;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GS_Shop.Home.Services.Services;

public class UserService : IUserService
{
    private readonly IRequestClient<LoginEvent> _requestClient;
    private readonly IRequestClient<RegisterEvent> _registerRequestClient;
    private readonly JwtSettings _jwtSettings;

    public UserService(IRequestClient<LoginEvent> requestClient, IOptions<JwtSettings> jwtSettings, IRequestClient<RegisterEvent> registerRequestClient)
    {
        _requestClient = requestClient;
        _registerRequestClient = registerRequestClient;
        _jwtSettings = jwtSettings.Value;
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