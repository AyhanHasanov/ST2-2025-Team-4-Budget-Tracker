using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models.Entities
{
    public class Transaction : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "BGN"; // ISO currency code

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)] // "Income" or "Expense"
        public string Type { get; set; } = "Expense";

        // Relationships
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public ApplicationUser User { get; set; } = null!;

        // Optional link to Budget (if applicable)
        public int? BudgetId { get; set; }
        public Budget? Budget { get; set; }

        // Account link
        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        // Optional link to Category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
