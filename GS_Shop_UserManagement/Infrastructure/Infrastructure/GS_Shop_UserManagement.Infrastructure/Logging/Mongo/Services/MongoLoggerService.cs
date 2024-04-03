using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;

public class MongoLoggerService(IMongoLoggerContext<AddLogDto> context,
    IHttpContextAccessor contextAccessor) : IMongoLoggerService
{
    public async Task AddLog(AddLogDto dto)
    {
        if (string.IsNullOrEmpty(dto.UserId))
        {
            var userIdClaim = contextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
                dto.UserId = userIdClaim.Value;
        }
        await context.Entity.InsertOneAsync(dto);
    }
}