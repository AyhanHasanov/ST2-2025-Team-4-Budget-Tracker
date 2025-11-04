using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Llm
{
    /// <summary>
    /// Request for budgeting advice from LLM
    /// </summary>
    public class AdviceRequestDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Question must be at least 5 characters")]
        [MaxLength(500, ErrorMessage = "Question must not exceed 500 characters")]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one expense is required")]
        public List<ExpenseItemDto> Expenses { get; set; } = new();
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget must be greater than 0")]
        public double Budget { get; set; }
    }
}

