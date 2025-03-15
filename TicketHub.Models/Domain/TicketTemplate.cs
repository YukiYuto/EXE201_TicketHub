using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class TicketTemplate
{
    [Key] public Guid TicketTemplateId { get; set; }

    [StringLength(150)] public string TicketName { get; set; } = null!;
    public Guid EventId { get; set; }
    [ForeignKey("EventId")] public virtual Event Event { get; set; } = null!;

    public string? ImageTicket { get; set; }
    public double TicketPrice { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    [StringLength(150)] public string Rank { get; set; } = null!;
    public bool IsValid { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<TicketSerialNumber> TicketSerialNumbers { get; set; } = new List<TicketSerialNumber>();
}