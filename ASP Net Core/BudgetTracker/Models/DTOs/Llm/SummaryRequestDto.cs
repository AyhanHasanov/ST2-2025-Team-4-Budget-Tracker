using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Llm
{
    /// <summary>
    /// Request for expense summarization from LLM
    /// </summary>
    public class SummaryRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one expense is required")]
        public List<ExpenseItemDto> Expenses { get; set; } = new();
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget must be greater than 0")]
        public double Budget { get; set; }
    }
}

