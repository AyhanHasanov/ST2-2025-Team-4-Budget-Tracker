using BudgetTracker.Models.Entities;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<Category>> GetAllWithParentCategoryAsync();
        Task<IEnumerable<Category>> GetAllByUserIdAsync(string userId);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByIdWithParentAsync(int id);
        Task<Category?> GetByIdWithSubCategoriesAsync(int id);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}

