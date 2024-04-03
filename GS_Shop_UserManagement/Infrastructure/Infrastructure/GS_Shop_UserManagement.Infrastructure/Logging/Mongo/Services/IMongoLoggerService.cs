using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.Services;

public interface IMongoLoggerService
{
    Task AddLog(AddLogDto dto);
}