using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Transaction
{
    public class UpdateTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters")]
        public string Currency { get; set; } = "BGN";

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("^(Income|Expense)$", ErrorMessage = "Type must be either 'Income' or 'Expense'")]
        public string Type { get; set; } = "Expense";

        [Required(ErrorMessage = "Account is required")]
        public int AccountId { get; set; }

        public int? CategoryId { get; set; }
        
        public int? BudgetId { get; set; }
    }
}
