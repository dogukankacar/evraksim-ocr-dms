using AutoMapper;
using DMS.Application.DTOs;
using DMS.Core.Common;
using DMS.Core.Interfaces;

namespace DMS.Application.Services;

public class AuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public AuditLogService(IAuditLogRepository auditLogRepository, IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var result = await _auditLogRepository.GetAllAsync(pageNumber, pageSize);
        return new PagedResult<AuditLogDto>
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Items = _mapper.Map<List<AuditLogDto>>(result.Items)
        };
    }

    public async Task<List<AuditLogDto>> GetByDocumentIdAsync(int documentId)
    {
        var logs = await _auditLogRepository.GetByDocumentIdAsync(documentId);
        return _mapper.Map<List<AuditLogDto>>(logs);
    }
}
