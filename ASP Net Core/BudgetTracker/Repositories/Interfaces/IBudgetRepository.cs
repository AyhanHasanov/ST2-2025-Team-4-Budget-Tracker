using BudgetTracker.Models.Entities;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetAllAsync();
        Task<IEnumerable<Budget>> GetAllByUserIdAsync(string userId);
        Task<Budget?> GetByIdAsync(int id);
        Task<Budget?> GetByIdWithTransactionsAsync(int id);
        Task<Budget> AddAsync(Budget budget);
        Task UpdateAsync(Budget budget);
        Task DeleteAsync(Budget budget);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}

