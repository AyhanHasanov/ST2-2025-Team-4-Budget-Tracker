using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.DTOs.Account
{
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "Account name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code (e.g., BGN, USD, EUR)")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters")]
        public string Currency { get; set; } = "BGN";

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal Balance { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}
