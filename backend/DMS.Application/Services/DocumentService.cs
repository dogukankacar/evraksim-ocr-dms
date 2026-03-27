using AutoMapper;
using DMS.Application.DTOs;
using DMS.Core.Common;
using DMS.Core.Entities;
using DMS.Core.Interfaces;

namespace DMS.Application.Services;

public class DocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOcrService _ocrService;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".docx", ".doc", ".jpg", ".jpeg", ".png"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public DocumentService(
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IOcrService ocrService,
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _ocrService = ocrService;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentDto>> GetAllAsync(int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        var result = await _documentRepository.GetAllAsync(pageNumber, pageSize, categoryId, searchTerm);
        return new PagedResult<DocumentDto>
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Items = _mapper.Map<List<DocumentDto>>(result.Items)
        };
    }

    public async Task<DocumentDto?> GetByIdAsync(int id, string userId)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null) return null;

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "View",
            EntityType = "Document",
            EntityId = id
        });

        return _mapper.Map<DocumentDto>(document);
    }

    public async Task<(DocumentDto? Data, string? Error)> UploadAsync(DocumentUploadDto dto, string userId)
    {
        var extension = Path.GetExtension(dto.File.FileName);
        if (!AllowedExtensions.Contains(extension))
            return (null, $"Desteklenmeyen dosya formatı: {extension}");

        if (dto.File.Length > MaxFileSize)
            return (null, "Dosya boyutu 10 MB'ı aşamaz.");

        var filePath = await _fileStorageService.SaveFileAsync(dto.File);

        var ocrContent = await _ocrService.ExtractTextAsync(filePath, dto.File.ContentType);

        var document = new Document
        {
            Title = dto.Title,
            Description = dto.Description,
            FilePath = filePath,
            OriginalFileName = dto.File.FileName,
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            OcrContent = ocrContent,
            CategoryId = dto.CategoryId,
            UploadedByUserId = userId
        };

        await _documentRepository.AddAsync(document);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "Upload",
            EntityType = "Document",
            EntityId = document.Id,
            Details = $"Dosya yüklendi: {dto.File.FileName}"
        });

        var result = await _documentRepository.GetByIdAsync(document.Id);
        return (_mapper.Map<DocumentDto>(result), null);
    }

    public async Task<(DocumentDto? Data, string? Error)> UpdateAsync(int id, DocumentUploadDto dto, string userId)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
            return (null, "Evrak bulunamadı.");

        var extension = Path.GetExtension(dto.File.FileName);
        if (!AllowedExtensions.Contains(extension))
            return (null, $"Desteklenmeyen dosya formatı: {extension}");

        if (dto.File.Length > MaxFileSize)
            return (null, "Dosya boyutu 10 MB'ı aşamaz.");

        // Mevcut versiyonu kaydet
        var version = new DocumentVersion
        {
            DocumentId = document.Id,
            VersionNumber = document.CurrentVersion,
            FilePath = document.FilePath,
            OcrContent = document.OcrContent,
            FileSize = document.FileSize,
            UploadedByUserId = document.UploadedByUserId
        };
        await _documentRepository.AddVersionAsync(version);

        // Yeni dosyayı kaydet
        var newFilePath = await _fileStorageService.SaveFileAsync(dto.File);
        var ocrContent = await _ocrService.ExtractTextAsync(newFilePath, dto.File.ContentType);

        document.Title = dto.Title;
        document.Description = dto.Description;
        document.FilePath = newFilePath;
        document.OriginalFileName = dto.File.FileName;
        document.ContentType = dto.File.ContentType;
        document.FileSize = dto.File.Length;
        document.OcrContent = ocrContent;
        document.CategoryId = dto.CategoryId;
        document.CurrentVersion++;

        await _documentRepository.UpdateAsync(document);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "Update",
            EntityType = "Document",
            EntityId = document.Id,
            Details = $"Yeni versiyon: {document.CurrentVersion}"
        });

        var result = await _documentRepository.GetByIdAsync(document.Id);
        return (_mapper.Map<DocumentDto>(result), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id, string userId)
    {
        var document = await _documentRepository.GetByIdWithVersionsAsync(id);
        if (document == null)
            return (false, "Evrak bulunamadı.");

        // Dosyaları sil
        await _fileStorageService.DeleteFileAsync(document.FilePath);
        foreach (var version in document.Versions)
            await _fileStorageService.DeleteFileAsync(version.FilePath);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "Delete",
            EntityType = "Document",
            EntityId = document.Id,
            Details = $"Evrak silindi: {document.Title}"
        });

        await _documentRepository.DeleteAsync(document);
        return (true, null);
    }

    public async Task<(byte[] FileBytes, string ContentType, string FileName)?> DownloadAsync(int id, string userId)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null) return null;

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "Download",
            EntityType = "Document",
            EntityId = id
        });

        return await _fileStorageService.GetFileAsync(document.FilePath);
    }

    public async Task<List<DocumentVersionDto>> GetVersionsAsync(int documentId)
    {
        var versions = await _documentRepository.GetVersionsAsync(documentId);
        return _mapper.Map<List<DocumentVersionDto>>(versions);
    }

    public async Task<(byte[] FileBytes, string ContentType, string FileName)?> DownloadVersionAsync(int documentId, int versionId, string userId)
    {
        var version = await _documentRepository.GetVersionAsync(documentId, versionId);
        if (version == null) return null;

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "Download",
            EntityType = "Document",
            EntityId = documentId,
            Details = $"Versiyon indirildi: {version.VersionNumber}"
        });

        return await _fileStorageService.GetFileAsync(version.FilePath);
    }

    public async Task<List<DocumentDto>> SearchAsync(string query)
    {
        var documents = await _documentRepository.SearchAsync(query);
        return _mapper.Map<List<DocumentDto>>(documents);
    }
}
