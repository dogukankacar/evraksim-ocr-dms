using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(categories));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), ApiResponse<CategoryDto>.SuccessResponse(category, "Kategori oluşturuldu."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        var (data, error) = await _categoryService.UpdateAsync(id, dto);
        if (error != null)
            return NotFound(ApiResponse<CategoryDto>.ErrorResponse(error));

        return Ok(ApiResponse<CategoryDto>.SuccessResponse(data!, "Kategori güncellendi."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _categoryService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse(error!));

        return Ok(ApiResponse<object>.SuccessResponse(null!, "Kategori silindi."));
    }
}
