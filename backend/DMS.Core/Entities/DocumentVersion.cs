namespace DMS.Core.Entities;

public class DocumentVersion
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public int VersionNumber { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? OcrContent { get; set; }
    public long FileSize { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public AppUser UploadedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
