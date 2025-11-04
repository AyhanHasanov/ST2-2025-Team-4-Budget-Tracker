using BudgetTracker.Models.DTOs.Llm;

namespace BudgetTracker.Services.Interfaces
{
    /// <summary>
    /// Service for communicating with the Python LLM microservice
    /// </summary>
    public interface ILlmService
    {
        /// <summary>
        /// Get a natural language summary of expenses from the LLM
        /// </summary>
        Task<SummaryResponseDto?> GetExpenseSummaryAsync(SummaryRequestDto request);
        
        /// <summary>
        /// Get budgeting advice from the LLM based on user's question and expenses
        /// </summary>
        Task<AdviceResponseDto?> GetBudgetingAdviceAsync(AdviceRequestDto request);
        
        /// <summary>
        /// Check if the LLM service is available
        /// </summary>
        Task<bool> IsServiceAvailableAsync();
    }
}

