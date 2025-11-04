using BudgetTracker.Models.DTOs.Budget;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BudgetService(
            IBudgetRepository budgetRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _budgetRepository = budgetRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<BudgetViewDto>> GetAllBudgetsAsync()
        {
            var budgets = await _budgetRepository.GetAllAsync();
            return budgets.Select(MapToViewDto);
        }

        public async Task<IEnumerable<BudgetViewDto>> GetBudgetsByUserIdAsync(string userId)
        {
            var budgets = await _budgetRepository.GetAllByUserIdAsync(userId);
            
            // Load transactions for each budget to calculate statistics
            var budgetsWithTransactions = new List<Budget>();
            foreach (var budget in budgets)
            {
                var budgetWithTransactions = await _budgetRepository.GetByIdWithTransactionsAsync(budget.Id);
                if (budgetWithTransactions != null)
                {
                    budgetsWithTransactions.Add(budgetWithTransactions);
                }
            }
            
            return budgetsWithTransactions.Select(MapToViewDto);
        }

        public async Task<BudgetViewDto?> GetBudgetByIdAsync(int id)
        {
            var budget = await _budgetRepository.GetByIdWithTransactionsAsync(id);
            return budget == null ? null : MapToViewDto(budget);
        }

        public async Task<BudgetViewDto> CreateBudgetAsync(CreateBudgetDto createDto, string userId)
        {
            var budget = new Budget
            {
                Name = createDto.Name,
                BudgetAmount = createDto.BudgetAmount,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                AccountId = createDto.AccountId,
                UserId = userId
            };

            var createdBudget = await _budgetRepository.AddAsync(budget);
            return MapToViewDto(createdBudget);
        }

        public async Task<bool> UpdateBudgetAsync(int id, UpdateBudgetDto updateDto)
        {
            var existingBudget = await _budgetRepository.GetByIdAsync(id);
            if (existingBudget == null)
            {
                return false;
            }

            existingBudget.Name = updateDto.Name;
            existingBudget.BudgetAmount = updateDto.BudgetAmount;
            existingBudget.StartDate = updateDto.StartDate;
            existingBudget.EndDate = updateDto.EndDate;
            existingBudget.AccountId = updateDto.AccountId;
            existingBudget.ModifiedAt = DateTime.UtcNow;

            await _budgetRepository.UpdateAsync(existingBudget);
            return true;
        }

        public async Task<bool> DeleteBudgetAsync(int id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null)
            {
                return false;
            }

            // Manually cascade delete: Delete all transactions linked to this budget
            // We set BudgetId to null instead of deleting transactions
            var transactions = await _transactionRepository.GetAllByBudgetIdAsync(id);
            foreach (var transaction in transactions)
            {
                transaction.BudgetId = null;
                await _transactionRepository.UpdateAsync(transaction);
            }

            await _budgetRepository.DeleteAsync(budget);
            return true;
        }

        public async Task<bool> BudgetExistsAsync(int id)
        {
            return await _budgetRepository.ExistsAsync(id);
        }

        public async Task RecalculateBudgetStatisticsAsync(int budgetId)
        {
            // This method is intentionally empty as budget statistics are calculated 
            // dynamically in MapToViewDto based on associated transactions.
            // The budget entity itself doesn't store these values.
            await Task.CompletedTask;
        }

        private BudgetViewDto MapToViewDto(Budget budget)
        {
            // Calculate statistics from transactions
            var transactions = budget.Transactions ?? new List<Transaction>();
            
            // Filter transactions within the budget period
            var relevantTransactions = transactions.Where(t => 
                t.Date >= budget.StartDate && t.Date <= budget.EndDate).ToList();
            
            var spentAmount = relevantTransactions
                .Where(t => t.Type == "Expense")
                .Sum(t => t.Amount);
            
            var incomeAmount = relevantTransactions
                .Where(t => t.Type == "Income")
                .Sum(t => t.Amount);
            
            var remainingAmount = budget.BudgetAmount - spentAmount;
            var exceeded = spentAmount > budget.BudgetAmount;
            var inLimit = !exceeded;
            
            // Get currency from linked account, default to "BGN"
            var currency = budget.Account?.Currency ?? "BGN";

            return new BudgetViewDto
            {
                Id = budget.Id,
                Name = budget.Name,
                BudgetAmount = budget.BudgetAmount,
                Currency = currency,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                AccountId = budget.AccountId,
                AccountName = budget.Account?.Name,
                CreatedAt = budget.CreatedAt,
                ModifiedAt = budget.ModifiedAt,

                // Calculated fields from transactions
                SpentAmount = spentAmount,
                IncomeAmount = incomeAmount,
                RemainingAmount = remainingAmount,
                InLimit = inLimit,
                Exceeded = exceeded
            };
        }
    }
}

