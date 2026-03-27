namespace DMS.Core.Interfaces;

public interface IOcrService
{
    Task<string?> ExtractTextAsync(string filePath, string contentType);
}
