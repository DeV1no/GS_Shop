using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.DTOs.User;
using GS_Shop_UserManagement.Application.Features.User.Requests.Commands;
using GS_Shop_UserManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GS_Shop_UserManagement.Application.Features.User.Handlers.Commands;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IUserRepository _repository;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public LoginUserHandler(IUserRepository repository, IMapper mapper,
        UserManager<Domain.Entities.User> userManager,
        IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByUserAndPassword(request.LoginDto.Password, request.LoginDto.UserName)
                   ?? throw new Exception("User NotFound");
        var jwtSecurityToken = GenerateToken(user);
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
            Claim= userClaim
        };
    }

    private JwtSecurityToken GenerateToken(Domain.Entities.User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        claims.AddRange(user.UserClaims.Select(userClaim => new Claim(userClaim.ClaimType, "true")));
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        claims.AddRange(user.UserClaimLimitations.Select(lClaim =>
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