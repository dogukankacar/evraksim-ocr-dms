using DMS.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DMS.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadPath;

    public FileStorageService(IWebHostEnvironment env, IConfiguration configuration)
    {
        var basePath = configuration["FileSettings:UploadPath"] ?? "wwwroot/uploads";
        _uploadPath = Path.IsPathRooted(basePath) ? basePath : Path.Combine(env.ContentRootPath, basePath);
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var now = DateTime.UtcNow;
        var directory = Path.Combine(_uploadPath, now.Year.ToString(), now.Month.ToString("D2"));
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(directory, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    public Task<(byte[] FileBytes, string ContentType, string FileName)> GetFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Dosya bulunamadı.", filePath);

        var bytes = File.ReadAllBytes(filePath);
        var contentType = GetContentType(filePath);
        var fileName = Path.GetFileName(filePath);

        return Task.FromResult((bytes, contentType, fileName));
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}
