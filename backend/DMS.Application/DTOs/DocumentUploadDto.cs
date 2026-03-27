using Microsoft.AspNetCore.Http;

namespace DMS.Application.DTOs;

public class DocumentUploadDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public IFormFile File { get; set; } = null!;
}
