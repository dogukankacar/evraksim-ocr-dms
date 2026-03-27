using DMS.Core.Common;
using DMS.Core.Entities;

namespace DMS.Core.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(int id);
    Task<Document?> GetByIdWithVersionsAsync(int id);
    Task<PagedResult<Document>> GetAllAsync(int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null);
    Task<List<Document>> SearchAsync(string query);
    Task<Document> AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Document document);
    Task AddVersionAsync(DocumentVersion version);
    Task<DocumentVersion?> GetVersionAsync(int documentId, int versionId);
    Task<List<DocumentVersion>> GetVersionsAsync(int documentId);
}
