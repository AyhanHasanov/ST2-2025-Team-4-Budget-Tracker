using BudgetTracker.Models.DTOs.Budget;

namespace BudgetTracker.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetViewDto>> GetAllBudgetsAsync();
        Task<IEnumerable<BudgetViewDto>> GetBudgetsByUserIdAsync(string userId);
        Task<BudgetViewDto?> GetBudgetByIdAsync(int id);
        Task<BudgetViewDto> CreateBudgetAsync(CreateBudgetDto createDto, string userId);
        Task<bool> UpdateBudgetAsync(int id, UpdateBudgetDto updateDto);
        Task<bool> DeleteBudgetAsync(int id);
        Task<bool> BudgetExistsAsync(int id);
        Task RecalculateBudgetStatisticsAsync(int budgetId);
    }
}

