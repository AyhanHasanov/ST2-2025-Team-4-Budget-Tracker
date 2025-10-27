using BudgetTracker.Models.DTOs.Transaction;

namespace BudgetTracker.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionViewDto>> GetAllTransactionsAsync();
        Task<IEnumerable<TransactionViewDto>> GetTransactionsByUserIdAsync(string userId);
        Task<IEnumerable<TransactionViewDto>> GetTransactionsByAccountIdAsync(int accountId);
        Task<IEnumerable<TransactionViewDto>> GetTransactionsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<TransactionViewDto>> GetTransactionsByBudgetIdAsync(int budgetId);
        Task<TransactionViewDto?> GetTransactionByIdAsync(int id);
        Task<TransactionViewDto?> CreateTransactionAsync(CreateTransactionDto createDto, string userId);
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto updateDto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<bool> TransactionExistsAsync(int id);
    }
}

