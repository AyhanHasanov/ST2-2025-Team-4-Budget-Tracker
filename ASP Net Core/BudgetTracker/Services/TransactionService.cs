using BudgetTracker.Models.DTOs.Transaction;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IBudgetRepository _budgetRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IBudgetRepository budgetRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<TransactionViewDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllWithDetailsAsync();
            return transactions.Select(MapToViewDto);
        }

        public async Task<IEnumerable<TransactionViewDto>> GetTransactionsByUserIdAsync(string userId)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId);
            return transactions.Select(MapToViewDto);
        }

        public async Task<IEnumerable<TransactionViewDto>> GetTransactionsByAccountIdAsync(int accountId)
        {
            var transactions = await _transactionRepository.GetAllByAccountIdAsync(accountId);
            return transactions.Select(MapToViewDto);
        }

        public async Task<IEnumerable<TransactionViewDto>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            var transactions = await _transactionRepository.GetAllByCategoryIdAsync(categoryId);
            return transactions.Select(MapToViewDto);
        }

        public async Task<IEnumerable<TransactionViewDto>> GetTransactionsByBudgetIdAsync(int budgetId)
        {
            var transactions = await _transactionRepository.GetAllByBudgetIdAsync(budgetId);
            return transactions.Select(MapToViewDto);
        }

        public async Task<TransactionViewDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdWithDetailsAsync(id);
            return transaction == null ? null : MapToViewDto(transaction);
        }

        public async Task<TransactionViewDto?> CreateTransactionAsync(CreateTransactionDto createDto, string userId)
        {
            // Get the account to update its balance
            var account = await _accountRepository.GetByIdAsync(createDto.AccountId);
            if (account == null)
            {
                return null; // Account not found
            }

            // Create the transaction
            var transaction = new Transaction
            {
                Amount = createDto.Amount,
                Date = createDto.Date,
                Description = createDto.Description,
                Type = createDto.Type,
                AccountId = createDto.AccountId,
                CategoryId = createDto.CategoryId,
                BudgetId = createDto.BudgetId,
                UserId = userId
            };

            // Update account balance
            UpdateAccountBalance(account, transaction.Amount, transaction.Type, isReversal: false);

            // Save transaction
            var createdTransaction = await _transactionRepository.AddAsync(transaction);

            // Retrieve with details for full DTO mapping
            var transactionWithDetails = await _transactionRepository.GetByIdWithDetailsAsync(createdTransaction.Id);
            return transactionWithDetails == null ? null : MapToViewDto(transactionWithDetails);
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto updateDto)
        {
            var existingTransaction = await _transactionRepository.GetByIdAsync(id);
            if (existingTransaction == null)
            {
                return false;
            }

            var oldAccount = await _accountRepository.GetByIdAsync(existingTransaction.AccountId);
            if (oldAccount == null)
            {
                return false; // Old account not found
            }

            // Reverse the old transaction's effect on the old account balance
            UpdateAccountBalance(oldAccount, existingTransaction.Amount, existingTransaction.Type, isReversal: true);

            // If the account is changing, get the new account
            if (existingTransaction.AccountId != updateDto.AccountId)
            {
                var newAccount = await _accountRepository.GetByIdAsync(updateDto.AccountId);
                if (newAccount == null)
                {
                    // Restore the old account balance since we're failing
                    UpdateAccountBalance(oldAccount, existingTransaction.Amount, existingTransaction.Type, isReversal: false);
                    return false; // New account not found
                }

                // Update transaction properties including account
                existingTransaction.AccountId = updateDto.AccountId;
                existingTransaction.Amount = updateDto.Amount;
                existingTransaction.Date = updateDto.Date;
                existingTransaction.Description = updateDto.Description;
                existingTransaction.Type = updateDto.Type;
                existingTransaction.CategoryId = updateDto.CategoryId;
                existingTransaction.BudgetId = updateDto.BudgetId;
                existingTransaction.ModifiedAt = DateTime.UtcNow;

                // Apply the new transaction's effect on the new account balance
                UpdateAccountBalance(newAccount, existingTransaction.Amount, existingTransaction.Type, isReversal: false);
            }
            else
            {
                // Same account, just update the transaction properties
                existingTransaction.Amount = updateDto.Amount;
                existingTransaction.Date = updateDto.Date;
                existingTransaction.Description = updateDto.Description;
                existingTransaction.Type = updateDto.Type;
                existingTransaction.CategoryId = updateDto.CategoryId;
                existingTransaction.BudgetId = updateDto.BudgetId;
                existingTransaction.ModifiedAt = DateTime.UtcNow;

                // Apply the new transaction's effect on the same account balance
                UpdateAccountBalance(oldAccount, existingTransaction.Amount, existingTransaction.Type, isReversal: false);
            }

            await _transactionRepository.UpdateAsync(existingTransaction);
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return false;
            }

            // Get the account and reverse the balance
            var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
            if (account != null)
            {
                UpdateAccountBalance(account, transaction.Amount, transaction.Type, isReversal: true);
            }

            await _transactionRepository.DeleteAsync(transaction);
            return true;
        }

        public async Task<bool> TransactionExistsAsync(int id)
        {
            return await _transactionRepository.ExistsAsync(id);
        }

        private void UpdateAccountBalance(Account account, decimal amount, string type, bool isReversal = false)
        {
            decimal factor = (type == "Income") ? 1 : -1;
            if (isReversal)
            {
                factor *= -1;
            }

            account.Balance += (amount * factor);
            account.ModifiedAt = DateTime.UtcNow;
        }

        private TransactionViewDto MapToViewDto(Transaction transaction)
        {
            return new TransactionViewDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Currency = transaction.Account?.Currency ?? "BGN",
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name ?? string.Empty,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,
                BudgetId = transaction.BudgetId,
                BudgetName = transaction.Budget?.Name,
                CreatedAt = transaction.CreatedAt,
                ModifiedAt = transaction.ModifiedAt
            };
        }
    }
}

