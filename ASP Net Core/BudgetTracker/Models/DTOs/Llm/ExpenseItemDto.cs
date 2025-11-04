using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Llm
{
    /// <summary>
    /// Represents a single expense item for LLM processing
    /// </summary>
    public class ExpenseItemDto
    {
        [Required]
        public string Category { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public double Amount { get; set; }
    }
}

