namespace DMS.Core.Entities;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? OcrContent { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string UploadedByUserId { get; set; } = string.Empty;
    public AppUser UploadedBy { get; set; } = null!;
    public int CurrentVersion { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
}
