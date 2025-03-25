namespace TicketHub.Models.DTO.Category;

public class GetCategoryByIdDto
{
    public GetCategoryByIdDto()
    {
        SubcategoryNames = new List<string>();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ParentCategoryName { get; set; } // Thêm thuộc tính mới  
    public List<string> SubcategoryNames { get; set; } // Danh sách tên các danh mục con  
}