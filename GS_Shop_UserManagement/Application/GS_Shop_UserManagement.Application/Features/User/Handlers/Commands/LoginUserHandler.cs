using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using GS_Shop_UserManagement.Application.Models;
using GS_Shop_UserManagement.Infrastructure.Redis;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IUserRepository _repository;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly IRedisCacheService _redisCacheService;


    public LoginUserHandler(IUserRepository repository, IMapper mapper,
        UserManager<Domain.Entities.User> userManager,
        IConfiguration configuration, IOptions<JwtSettings> jwtSettings, IRedisCacheService redisCacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _redisCacheService = redisCacheService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByUserAndPassword(request.LoginDto.Password, request.LoginDto.UserName)
                   ?? throw new Exception("User NotFound");
        var jwtSecurityToken = await GenerateToken(user);
        var claimLimitation = _mapper.Map<IList<UserClaimLimitationDto>>(user.UserClaimLimitations);
        var userClaim = _mapper.Map<IList<UserClaimDto>>(user.UserClaims);

        return new LoginResponseDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            ExpiresAt = DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
            ClaimLimitation = claimLimitation,
            Claim = userClaim
        };
    }

    private async Task<JwtSecurityToken> GenerateToken(Domain.Entities.User user)
    {
        

        var userPermissions = user.UserClaims
            .Select(userClaim => new Claim(userClaim.ClaimType, "true"));
        var userRoles = user.Roles
            .Select(role => new Claim(ClaimTypes.Role, role.Name));
        var userLimitation = user.UserClaimLimitations
            .Select(lClaim =>
                new Claim(lClaim.ClaimLimitationValue,
                    lClaim.LimitedIds + "," + lClaim.LimitationField));

        var permissions = userPermissions as Claim[] ?? userPermissions.ToArray();
        var roles = userRoles as Claim[] ?? userRoles.ToArray();
        var limitations = userLimitation as Claim[] ?? userLimitation.ToArray();
        var convertedToJson = JsonConvert.SerializeObject(new
        {
            permissions,
            roles ,
            limitations
        });
       
        var randomKey = GenerateRandomKey();
        await _redisCacheService.SetAsync(randomKey, convertedToJson, TimeSpan.FromHours(4));
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        claims.Add(
            new Claim("redisKey",randomKey)
        );
        claims.AddRange(userPermissions);
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
    
    private string GenerateRandomKey()
    {
        // Implement logic to generate a random key (e.g., using Guid)
        return Guid.NewGuid().ToString();
    }

}