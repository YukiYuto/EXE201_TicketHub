using System.ComponentModel.DataAnnotations;

namespace TicketHub.Models.Domain;

public class Event : BaseEntity<string, string, int>
{
    [Key] public Guid EventId { get; set; }
    [StringLength(100)] public string EventName { get; set; } = null!;
    [StringLength(500)] public string EventDescription { get; set; } = null!;
    public DateTime EventDate { get; set; }
    [StringLength(1000)] public string? EventImage { get; set; }
    [StringLength(100)]public string City { get; set; } = null!;
    [StringLength(100)]public string District { get; set; } = null!;
    [StringLength(100)]public string Address { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = null!;
}