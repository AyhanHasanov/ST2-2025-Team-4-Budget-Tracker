using BudgetTracker.Models.DTOs.Category;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryViewDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllWithParentCategoryAsync();
            return categories.Select(MapToViewDto);
        }

        public async Task<IEnumerable<CategoryViewDto>> GetCategoriesByUserIdAsync(string userId)
        {
            var categories = await _categoryRepository.GetAllByUserIdAsync(userId);
            return categories.Select(MapToViewDto);
        }

        public async Task<CategoryViewDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithParentAsync(id);
            return category == null ? null : MapToViewDto(category);
        }

        public async Task<CategoryViewDto> CreateCategoryAsync(CreateCategoryDto createDto, string userId)
        {
            var category = new Category
            {
                Name = createDto.Name,
                Description = createDto.Description,
                ParentCategoryId = createDto.ParentCategoryId,
                UserId = userId
            };

            var createdCategory = await _categoryRepository.AddAsync(category);
            return MapToViewDto(createdCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = updateDto.Name;
            existingCategory.Description = updateDto.Description;
            existingCategory.ParentCategoryId = updateDto.ParentCategoryId;
            existingCategory.ModifiedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existingCategory);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _categoryRepository.ExistsAsync(id);
        }

        private CategoryViewDto MapToViewDto(Category category)
        {
            return new CategoryViewDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                CreatedAt = category.CreatedAt,
                ModifiedAt = category.ModifiedAt,
                SubCategories = null // Avoid circular references
            };
        }
    }
}

