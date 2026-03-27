using DMS.Core.Common;
using DMS.Core.Entities;

namespace DMS.Core.Interfaces;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLog>> GetAllAsync(int pageNumber, int pageSize);
    Task<List<AuditLog>> GetByDocumentIdAsync(int documentId);
    Task AddAsync(AuditLog log);
}
