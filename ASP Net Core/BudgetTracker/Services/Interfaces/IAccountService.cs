using BudgetTracker.Models.DTOs.Account;

namespace BudgetTracker.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountViewDto>> GetAllAccountsAsync();
        Task<IEnumerable<AccountViewDto>> GetAccountsByUserIdAsync(string userId);
        Task<AccountViewDto?> GetAccountByIdAsync(int id);
        Task<AccountViewDto> CreateAccountAsync(CreateAccountDto createDto, string userId);
        Task<bool> UpdateAccountAsync(int id, UpdateAccountDto updateDto);
        Task<bool> DeleteAccountAsync(int id);
        Task<bool> AccountExistsAsync(int id);
    }
}

