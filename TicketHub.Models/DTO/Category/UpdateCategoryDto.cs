namespace TicketHub.Models.DTO.Category;

public class UpdateCategoryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; }
}