namespace BudgetTracker.Models.DTOs.Transaction
{
    public class TransactionViewDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BGN";
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        
        // Navigation properties (display names)
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? BudgetId { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
