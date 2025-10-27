using BudgetTracker.Models.DTOs.Category;

namespace BudgetTracker.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryViewDto>> GetCategoriesByUserIdAsync(string userId);
        Task<CategoryViewDto?> GetCategoryByIdAsync(int id);
        Task<CategoryViewDto> CreateCategoryAsync(CreateCategoryDto createDto, string userId);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}

