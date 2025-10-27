using BudgetTracker.Models.DTOs.Account;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<AccountViewDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(MapToViewDto);
        }

        public async Task<IEnumerable<AccountViewDto>> GetAccountsByUserIdAsync(string userId)
        {
            var accounts = await _accountRepository.GetAllByUserIdAsync(userId);
            return accounts.Select(MapToViewDto);
        }

        public async Task<AccountViewDto?> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return account == null ? null : MapToViewDto(account);
        }

        public async Task<AccountViewDto> CreateAccountAsync(CreateAccountDto createDto, string userId)
        {
            var account = new Account
            {
                Name = createDto.Name,
                Currency = createDto.Currency,
                Balance = createDto.Balance,
                Description = createDto.Description,
                UserId = userId
            };

            var createdAccount = await _accountRepository.AddAsync(account);
            return MapToViewDto(createdAccount);
        }

        public async Task<bool> UpdateAccountAsync(int id, UpdateAccountDto updateDto)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return false;
            }

            existingAccount.Name = updateDto.Name;
            existingAccount.Currency = updateDto.Currency;
            existingAccount.Balance = updateDto.Balance;
            existingAccount.Description = updateDto.Description;
            existingAccount.ModifiedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(existingAccount);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return false;
            }

            await _accountRepository.DeleteAsync(account);
            return true;
        }

        public async Task<bool> AccountExistsAsync(int id)
        {
            return await _accountRepository.ExistsAsync(id);
        }

        private AccountViewDto MapToViewDto(Account account)
        {
            return new AccountViewDto
            {
                Id = account.Id,
                Name = account.Name,
                Currency = account.Currency,
                Balance = account.Balance,
                Description = account.Description,
                CreatedAt = account.CreatedAt,
                ModifiedAt = account.ModifiedAt
            };
        }
    }
}

