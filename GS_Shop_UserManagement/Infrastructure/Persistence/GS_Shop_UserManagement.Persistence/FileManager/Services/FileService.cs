using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Persistence.FileManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace GS_Shop_UserManagement.Persistence.FileManager.Services;

public class FileService : IFileService
{
    private readonly GSShopUserManagementDbContext _context;

    public FileService(GSShopUserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<string> PostFileAsync(IFormFile fileData)
    {
        try
        {
            var fileDetails = new FileDetails()
            {
                FileName = fileData.FileName,
            };

            using (var stream = new MemoryStream())
            {
                await fileData.CopyToAsync(stream);
                fileDetails.FileData = stream.ToArray();
            }
            var fileName = await SaveFileToPath(fileData);
            fileDetails.FileName = fileName;
            _context.FileDetails.Add(fileDetails);
            await _context.SaveChangesAsync();
            return fileName;

        }
        catch (Exception ex)
        {
            // Log or handle the exception
            throw new Exception("Error occurred while saving file.", ex);
        }
    }


    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        try
        {
            foreach (var file in fileData)
            {
                var fileDetails = new FileDetails()
                {
                    Id = 0,
                    FileName = file.FileDetails.FileName,
                };

                using (var stream = new MemoryStream())
                {
                    await file.FileDetails.CopyToAsync(stream);
                    fileDetails.FileData = stream.ToArray();
                }

                _context.FileDetails.Add(fileDetails);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> DownloadFileById(int Id)
    {
        try
        {
            var file = await _context.FileDetails.FindAsync(Id);

            if (file?.FileData != null)
            {
                var content = new MemoryStream(file.FileData);
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "../FileDownload", fileName);

                await CopyStream(content, path);

                // Return the file name
                return fileName;
            }
            else
            {
                throw new FileNotFoundException("File not found in the database.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while downloading file by Id.", ex);
        }
    }


    private async Task<string> SaveFileToPath(IFormFile fileData)
    {
        try
        {
            const string directoryPath = "../FileDownload"; // Relative directory path
            var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);

            // Check if the directory exists, and if not, create it
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            // Generate a unique file name to avoid conflicts
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileData.FileName);
            var filePath = Path.Combine(absolutePath, fileName);

            // Save the file
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fileData.CopyToAsync(fileStream);

            // Return the generated file name
            return fileName;
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            throw new Exception("Error occurred while saving file to path.", ex);
        }
    }



    private async Task CopyStream(Stream stream, string downloadPath)
    {
        await using var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
    }
}
