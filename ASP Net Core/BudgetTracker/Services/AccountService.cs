using BudgetTracker.Models.DTOs.Account;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AccountService(
            IAccountRepository accountRepository,
            IBudgetRepository budgetRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
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

            // Manually cascade delete: Delete all transactions for this account
            var transactions = await _transactionRepository.GetAllByAccountIdAsync(id);
            foreach (var transaction in transactions)
            {
                await _transactionRepository.DeleteAsync(transaction);
            }

            // Manually cascade delete: Delete all budgets for this account
            var budgets = await _budgetRepository.GetAllAsync();
            var accountBudgets = budgets.Where(b => b.AccountId == id).ToList();
            foreach (var budget in accountBudgets)
            {
                // First delete all transactions linked to this budget
                var budgetTransactions = await _transactionRepository.GetAllByBudgetIdAsync(budget.Id);
                foreach (var transaction in budgetTransactions)
                {
                    await _transactionRepository.DeleteAsync(transaction);
                }
                
                // Then delete the budget
                await _budgetRepository.DeleteAsync(budget);
            }

            // Finally, delete the account
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

