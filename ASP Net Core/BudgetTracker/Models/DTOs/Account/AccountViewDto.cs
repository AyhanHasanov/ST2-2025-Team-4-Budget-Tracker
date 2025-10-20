namespace BudgetTracker.Models.DTOs.Account
{
    public class AccountViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "BGN";
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
