namespace BudgetTracker.Models.DTOs.Llm
{
    /// <summary>
    /// Response from LLM advice generation
    /// </summary>
    public class AdviceResponseDto
    {
        public string Advice { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }
}

