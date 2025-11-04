namespace BudgetTracker.Models.DTOs.Llm
{
    /// <summary>
    /// Response from LLM summarization
    /// </summary>
    public class SummaryResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
    }
}

