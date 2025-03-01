using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class Event : BaseEntity<string, string, int>
{
    [Key] public Guid EventId { get; set; }
    [StringLength(100)] public string EventName { get; set; } = null!;
    [StringLength(500)] public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }
    [StringLength(1000)] public string? EventImage { get; set; }
    [StringLength(1000)]public string Location { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<TicketTemplate> TicketTemplates { get; set; } = new List<TicketTemplate>();
}