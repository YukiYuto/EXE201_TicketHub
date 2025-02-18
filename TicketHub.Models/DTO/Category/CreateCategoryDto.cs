using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.DTO.Category;

public class CreateCategoryDto
{
    [StringLength(50)] public string CategoryName { get; set; } = null!;
    public string? ParentCategoryId { get; set; }
}