using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GS_Shop_UserManagement.Persistence.Minio.ServiceModels;

public record UploadFileServiceModel(IFormFile FileContent,string FileType,string Bucket);
public record UploadFileServiceModelResult(string FileName,string BucketName);