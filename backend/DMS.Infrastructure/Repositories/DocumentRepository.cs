using DMS.Core.Common;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Document?> GetByIdWithVersionsAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<PagedResult<Document>> GetAllAsync(int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        var query = _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(d => d.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(d =>
                d.Title.ToLower().Contains(term) ||
                (d.Description != null && d.Description.ToLower().Contains(term)) ||
                (d.OcrContent != null && d.OcrContent.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Document>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<List<Document>> SearchAsync(string queryText)
    {
        var term = queryText.ToLower();
        return await _context.Documents
            .Include(d => d.Category)
            .Include(d => d.UploadedBy)
            .Where(d =>
                d.Title.ToLower().Contains(term) ||
                (d.Description != null && d.Description.ToLower().Contains(term)) ||
                (d.OcrContent != null && d.OcrContent.ToLower().Contains(term)) ||
                d.OriginalFileName.ToLower().Contains(term))
            .OrderByDescending(d => d.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task<Document> AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task UpdateAsync(Document document)
    {
        document.UpdatedAt = DateTime.UtcNow;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Document document)
    {
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task AddVersionAsync(DocumentVersion version)
    {
        _context.DocumentVersions.Add(version);
        await _context.SaveChangesAsync();
    }

    public async Task<DocumentVersion?> GetVersionAsync(int documentId, int versionId)
    {
        return await _context.DocumentVersions
            .Include(v => v.UploadedBy)
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.Id == versionId);
    }

    public async Task<List<DocumentVersion>> GetVersionsAsync(int documentId)
    {
        return await _context.DocumentVersions
            .Include(v => v.UploadedBy)
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }
}
