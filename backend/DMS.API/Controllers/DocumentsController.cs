using System.Security.Claims;
using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocumentsController(DocumentService documentService)
    {
        _documentService = documentService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null, [FromQuery] string? searchTerm = null)
    {
        var result = await _documentService.GetAllAsync(pageNumber, pageSize, categoryId, searchTerm);
        return Ok(ApiResponse<PagedResult<DocumentDto>>.SuccessResponse(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var document = await _documentService.GetByIdAsync(id, UserId);
        if (document == null) return NotFound(ApiResponse<DocumentDto>.ErrorResponse("Evrak bulunamadı."));

        return Ok(ApiResponse<DocumentDto>.SuccessResponse(document));
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
    {
        var (data, error) = await _documentService.UploadAsync(dto, UserId);
        if (error != null)
            return BadRequest(ApiResponse<DocumentDto>.ErrorResponse(error));

        return CreatedAtAction(nameof(GetById), new { id = data!.Id }, ApiResponse<DocumentDto>.SuccessResponse(data, "Evrak yüklendi."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] DocumentUploadDto dto)
    {
        var (data, error) = await _documentService.UpdateAsync(id, dto, UserId);
        if (error != null)
            return BadRequest(ApiResponse<DocumentDto>.ErrorResponse(error));

        return Ok(ApiResponse<DocumentDto>.SuccessResponse(data!, "Evrak güncellendi."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _documentService.DeleteAsync(id, UserId);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse(error!));

        return Ok(ApiResponse<object>.SuccessResponse(null!, "Evrak silindi."));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _documentService.DownloadAsync(id, UserId);
        if (result == null) return NotFound(ApiResponse<object>.ErrorResponse("Dosya bulunamadı."));

        var (fileBytes, contentType, fileName) = result.Value;
        return File(fileBytes, contentType, fileName);
    }

    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersions(int id)
    {
        var versions = await _documentService.GetVersionsAsync(id);
        return Ok(ApiResponse<List<DocumentVersionDto>>.SuccessResponse(versions));
    }

    [HttpGet("{id}/versions/{versionId}/download")]
    public async Task<IActionResult> DownloadVersion(int id, int versionId)
    {
        var result = await _documentService.DownloadVersionAsync(id, versionId, UserId);
        if (result == null) return NotFound(ApiResponse<object>.ErrorResponse("Versiyon bulunamadı."));

        var (fileBytes, contentType, fileName) = result.Value;
        return File(fileBytes, contentType, fileName);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResponse<object>.ErrorResponse("Arama terimi gerekli."));

        var results = await _documentService.SearchAsync(q);
        return Ok(ApiResponse<List<DocumentDto>>.SuccessResponse(results));
    }
}
