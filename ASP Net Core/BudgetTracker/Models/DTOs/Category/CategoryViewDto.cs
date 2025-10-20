namespace BudgetTracker.Models.DTOs.Category
{
    public class CategoryViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        
        // For hierarchical display
        public ICollection<CategoryViewDto>? SubCategories { get; set; }
    }
}

