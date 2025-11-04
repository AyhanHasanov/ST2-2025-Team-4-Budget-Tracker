using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models.Entities
{
    public class Budget : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;  // Budget name (e.g., "Monthly Budget", "Q1 2024")

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal BudgetAmount { get; set; }  // Max budget for this period

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }   // Period start

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }     // Period end

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public int? AccountId { get; set; }  // Optional: Link budget to specific account
        
        public ApplicationUser User { get; set; } = null!;
        public Account? Account { get; set; }  // Navigation property
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
