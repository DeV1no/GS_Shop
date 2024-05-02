namespace GS_Shop_UserManagement.Infrastructure.Helpers;

public static class BucketPathSeparator
{
    public static Tuple<string,string>  Separat(string downloadLink)
    {
        var parts = downloadLink.Split('/');
        var bucket = parts[0];
      var  fileName = Path.GetFileName(downloadLink);
      return new Tuple<string, string>(bucket, fileName);
    }
}