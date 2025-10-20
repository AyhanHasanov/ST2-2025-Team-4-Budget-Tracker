using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Budget
{
    public class CreateBudgetDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0")]
        public decimal BudgetAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        
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
