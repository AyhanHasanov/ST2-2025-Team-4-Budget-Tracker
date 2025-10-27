using BudgetTracker.Models.Entities;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<IEnumerable<Transaction>> GetAllWithDetailsAsync();
        Task<IEnumerable<Transaction>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<Transaction>> GetAllByAccountIdAsync(int accountId);
        Task<IEnumerable<Transaction>> GetAllByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Transaction>> GetAllByBudgetIdAsync(int budgetId);
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetByIdWithDetailsAsync(int id);
        Task<Transaction> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}

