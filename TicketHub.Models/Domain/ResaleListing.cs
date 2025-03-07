using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.Models.Domain;

public class ResaleListing : BaseEntity<string, string, string>
{
    [Key] public Guid ResaleListingId { get; set; }

    public Guid TicketId { get; set; }

    [ForeignKey("TicketId")] public virtual Ticket Ticket { get; set; } = null!;

    public Guid CustomerId { get; set; }

    [ForeignKey("CustomerId")] public virtual Customer Customer { get; set; } = null!;

    public double Price { get; set; }

    public bool NegotiationStatus { get; set; }

    public bool IsSold { get; set; } = false;
}