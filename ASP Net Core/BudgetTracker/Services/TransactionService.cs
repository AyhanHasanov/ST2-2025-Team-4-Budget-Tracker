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

        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
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
                Currency = createDto.Currency,
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

            var account = await _accountRepository.GetByIdAsync(existingTransaction.AccountId);
            if (account == null)
            {
                return false; // Account not found
            }

            // Reverse the old transaction's effect on the account balance
            UpdateAccountBalance(account, existingTransaction.Amount, existingTransaction.Type, isReversal: true);

            // Update transaction properties
            existingTransaction.Amount = updateDto.Amount;
            existingTransaction.Currency = updateDto.Currency;
            existingTransaction.Date = updateDto.Date;
            existingTransaction.Description = updateDto.Description;
            existingTransaction.Type = updateDto.Type;
            existingTransaction.ModifiedAt = DateTime.UtcNow;

            // Apply the new transaction's effect on the account balance
            UpdateAccountBalance(account, existingTransaction.Amount, existingTransaction.Type, isReversal: false);

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
                Currency = transaction.Currency,
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name ?? string.Empty,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,
                BudgetId = transaction.BudgetId,
                CreatedAt = transaction.CreatedAt,
                ModifiedAt = transaction.ModifiedAt
            };
        }
    }
}

