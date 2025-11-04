namespace BudgetTracker.Models.DTOs.Budget
{
    public class BudgetViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public string Currency { get; set; } = "BGN";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public int? AccountId { get; set; }
        public string? AccountName { get; set; }

        // Calculated fields (logic will be incorporated in service/controller)
        public decimal SpentAmount { get; set; }
        public decimal IncomeAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool InLimit { get; set; }
        public bool Exceeded { get; set; }
    }
}
