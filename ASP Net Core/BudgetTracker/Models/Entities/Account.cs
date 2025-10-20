using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Models.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty; // e.g., "Main Checking", "Wallet", "PayPal"

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "BGN"; // default currency (ISO code)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0; // current balance in account

        [StringLength(500)]
        public string? Description { get; set; } // optional notes

        [Required]
        [StringLength(450)] // Standard ASP.NET Identity user ID length
        public string UserId { get; set; } = string.Empty; // link to ApplicationUser
        
        public ApplicationUser User { get; set; } = null!;

        // Navigation: transactions related to this account
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
