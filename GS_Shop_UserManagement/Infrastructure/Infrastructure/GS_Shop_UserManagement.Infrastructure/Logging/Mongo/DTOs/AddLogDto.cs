using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;

public class AddLogDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    [BsonElement("UserId")]
    public string UserId { get; set; } = string.Empty;
    [BsonElement("EndPointType")]
    public EntityEndPointType EndPointType { get; set; } = new EntityEndPointType();
    [BsonElement("EntityType")]
    public string EntityType { get; set; } = string.Empty;
    [BsonElement("RequestDeserialized")]
    public string RequestDeserialized { get; set; } = string.Empty;
    [BsonElement("ResponseDeserialized")]
    public string ResponseDeserialized { get; set; } = string.Empty;
    [BsonElement("DateTime")]
    public DateTime DateTime { get; } = DateTime.UtcNow;
}

public enum EntityEndPointType
{
    Get = 1, Post = 2, Put = 3, Delete = 4,
}