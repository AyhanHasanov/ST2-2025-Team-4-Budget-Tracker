using BudgetTracker.Models.Entities;

namespace BudgetTracker.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<Account>> GetAllByUserIdAsync(string userId);
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByIdWithTransactionsAsync(int id);
        Task<Account> AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}

