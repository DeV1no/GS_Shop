namespace GS_Shop_UserManagement.Persistence.Minio.ServiceModels;

public record GetObjectDownloadLinkServiceModel(string DownloadLink);
public record GetObjectDownloadLinkRequestModel(string FileName,string BucketName);
