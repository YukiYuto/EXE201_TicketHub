using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Category : BaseEntity<string, string, int>
{
    [Key] public Guid CategoryId { get; set; }
    [StringLength(50)] public string CategoryName { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public virtual Category? ParentCategory { get; set; }
    
    [NotMapped] public List<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}