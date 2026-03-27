using AutoMapper;
using DMS.Application.DTOs;
using DMS.Core.Entities;
using DMS.Core.Interfaces;

namespace DMS.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<(CategoryDto? Data, string? Error)> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return (null, "Kategori bulunamadı.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        await _categoryRepository.UpdateAsync(category);
        return (_mapper.Map<CategoryDto>(category), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null) return (false, "Kategori bulunamadı.");

        await _categoryRepository.DeleteAsync(category);
        return (true, null);
    }
}
