using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Data;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services
{
    public class MongoLoggerService : IMongoLoggerService
    {
        private readonly IMongoLoggerContext<AddLogDto> _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public MongoLoggerService(IMongoLoggerContext<AddLogDto> context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public void AddLog(AddLogDto dto)
        {
            if (string.IsNullOrEmpty(dto.UserId))
            {
                var userIdClaim = _contextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                    dto.UserId = userIdClaim.Value;
            }

            BackgroundJob.Enqueue(() => InsertLogToMongo(dto));
        }

        public void InsertLogToMongo(AddLogDto dto)
        {
            _context.Entity.InsertOne(dto);
        }
    }
}