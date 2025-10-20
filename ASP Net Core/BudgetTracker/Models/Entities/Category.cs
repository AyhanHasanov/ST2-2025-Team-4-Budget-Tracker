using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Nullable for root categories
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        // User relationship
        [Required]
        [StringLength(450)] // Standard ASP.NET Identity user ID length
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        // Navigation properties
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
