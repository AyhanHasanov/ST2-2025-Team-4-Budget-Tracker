using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Audit fields (from BaseEntity pattern)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; }

        // User profile information
        [PersonalData]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [PersonalData]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string PreferredCurrency { get; set; } = "BGN"; // ISO currency code

        [Required]
        [StringLength(10, MinimumLength = 2)]
        public string Language { get; set; } = "bg"; // Language code (e.g., "en", "bg")

        // Navigation properties
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        //public ICollection<AI_Log> AI_Logs { get; set; } = new List<AI_Log>();
    }
}
