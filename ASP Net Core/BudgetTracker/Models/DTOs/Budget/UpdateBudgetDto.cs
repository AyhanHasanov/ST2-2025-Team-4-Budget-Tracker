using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Budget
{
    public class UpdateBudgetDto
    {
        [Required(ErrorMessage = "Budget name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0")]
        public decimal BudgetAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int? AccountId { get; set; }  // Optional: Link to specific account

        // Custom validation method
        public bool IsValid(out string errorMessage)
        {
            if (EndDate <= StartDate)
            {
                errorMessage = "End date must be after start date";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }
}
