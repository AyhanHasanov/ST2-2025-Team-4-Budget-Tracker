using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models.Entities
{
    public class Budget : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal BudgetAmount { get; set; }  // Max budget for this period

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "BGN"; // ISO currency code

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }   // Period start

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }     // Period end

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
