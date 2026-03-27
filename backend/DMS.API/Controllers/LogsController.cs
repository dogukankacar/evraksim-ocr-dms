using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
    private readonly AuditLogService _auditLogService;

    public LogsController(AuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _auditLogService.GetAllAsync(pageNumber, pageSize);
        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result));
    }

    [HttpGet("document/{documentId}")]
    public async Task<IActionResult> GetByDocumentId(int documentId)
    {
        var logs = await _auditLogService.GetByDocumentIdAsync(documentId);
        return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(logs));
    }
}
