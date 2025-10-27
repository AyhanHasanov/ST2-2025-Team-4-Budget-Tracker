using BudgetTracker.Models.DTOs.Budget;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using BudgetTracker.Services.Interfaces;

namespace BudgetTracker.Services.Implementations
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<BudgetViewDto>> GetAllBudgetsAsync()
        {
            var budgets = await _budgetRepository.GetAllAsync();
            return budgets.Select(MapToViewDto);
        }

        public async Task<IEnumerable<BudgetViewDto>> GetBudgetsByUserIdAsync(string userId)
        {
            var budgets = await _budgetRepository.GetAllByUserIdAsync(userId);
            return budgets.Select(MapToViewDto);
        }

        public async Task<BudgetViewDto?> GetBudgetByIdAsync(int id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            return budget == null ? null : MapToViewDto(budget);
        }

        public async Task<BudgetViewDto> CreateBudgetAsync(CreateBudgetDto createDto, string userId)
        {
            var budget = new Budget
            {
                BudgetAmount = createDto.BudgetAmount,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                UserId = userId,
                Currency = "BGN" // Default currency
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

            existingBudget.BudgetAmount = updateDto.BudgetAmount;
            existingBudget.StartDate = updateDto.StartDate;
            existingBudget.EndDate = updateDto.EndDate;
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

            await _budgetRepository.DeleteAsync(budget);
            return true;
        }

        public async Task<bool> BudgetExistsAsync(int id)
        {
            return await _budgetRepository.ExistsAsync(id);
        }

        private BudgetViewDto MapToViewDto(Budget budget)
        {
            return new BudgetViewDto
            {
                Id = budget.Id,
                BudgetAmount = budget.BudgetAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CreatedAt = budget.CreatedAt,
                ModifiedAt = budget.ModifiedAt,

                // Default values for calculated fields
                SpentAmount = 0,
                IncomeAmount = 0,
                RemainingAmount = 0,
                InLimit = true,
                Exceeded = false
            };
        }
    }
}

