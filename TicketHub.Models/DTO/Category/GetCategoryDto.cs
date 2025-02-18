using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Category;

public class GetCategoryDto
{
    public Guid CategoryId { get; set; }
    [StringLength(50)] public string CategoryName { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; }
}