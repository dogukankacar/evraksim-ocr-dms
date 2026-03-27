using Microsoft.AspNetCore.Http;

namespace DMS.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file);
    Task DeleteFileAsync(string filePath);
    Task<(byte[] FileBytes, string ContentType, string FileName)> GetFileAsync(string filePath);
}
